using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Moq;
using JobManagementApi.Data;
using JobManagementApi.Services;
using JobManagementApi.Exceptions;
using JobManagementApi.Dtos.JobApplication;
using JobManagementApi.Enums;
using JobManagementApi.Interfaces;
using JobManagementApi.RequestHelpers.Pagination;

namespace JobManagementApi.Tests.Services;

public class JobApplicationServiceTests
{
    public Mock<ILogger<JobApplicationService>> logger=new();
    public Mock<ICurrentUserService> currentUser=new();

    public CreateJobApplicationDto createJobApplicationDto=new("resumeurl");

    public IConfigurationRoot configuration;
    public DapperContext context;

    public PaginationParams pagination = new()
    {
        PageNumber=1,
        PageSize=10,
    };

    public JobApplicationServiceTests()
    {
        configuration=new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        context=new DapperContext(configuration);
    }

    [Fact]
    public async Task CreateApplication_WhenUserIsUnAuthenticated_ThrowsUnauthorizedException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.CreateApplication(1,createJobApplicationDto));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task CreateApplication_WhenUserIsCandidate_ThrowsForbiddenException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.CreateApplication(1,createJobApplicationDto));
        Assert.Equal("Only candidate can apply",exception.Message);
    }

    [Fact]
    public async Task DeleteApplication_WhenUserIsUnAuthenticated_ThrowsUnauthorizedException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.DeleteApplication(1));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task DeleteApplication_WhenUserIsRecruiter_ThrowsForbiddenException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.DeleteApplication(1));
        Assert.Equal("Only candidate and admin can delete an application",exception.Message);
    }

    [Fact]
    public async Task UpdateApplicationStatus_WhenUserIsUnAuthenticated_ThrowsUnauthorizedException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.UpdateApplicationStatus(1,new (ApplicationStatus.Applied)));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task UpdateApplicationStatus_WhenUserIsCandidate_ThrowsForbiddenException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.UpdateApplicationStatus(1,new (ApplicationStatus.Applied)));
        Assert.Equal("Only recruiter and admin can delete an application",exception.Message);
    }

    [Fact]
    public async Task CreateApplication_WhenJobDoesNotExist_ThrowsNotFoundException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);
        CreateJobApplicationDto dto=new("url");
        
        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.CreateApplication(100,dto));

        Assert.Equal("Job not found",exception.Message);
    }

    [Fact]
    public async Task CreateApplication_WhenCandidateAlreadyApplied_ThrowsConflictException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.UserId).Returns(6);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);

        var exception=await Assert.ThrowsAsync<ConflictException>(()=>service.CreateApplication(4,createJobApplicationDto));

        Assert.Equal("Already applied to this job",exception.Message);
    }

    [Fact]
    public async Task ApplyForJob_WhenValidRequest_CreatesApplicationSuccessfully()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.UserId).Returns(6);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var result=await service.CreateApplication(3,createJobApplicationDto);
        
        Assert.Equal(result.ResumeUrl,createJobApplicationDto.ResumeUrl);
        Assert.Equal(6,result.CandidateId);
        Assert.Equal(3,result.JobId);

        await service.DeleteApplication(result.Id);
    }

    [Fact]
    public async Task DeleteApplication_WhenApplicationDoesNotExist_ThrowsNotFoundException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.UserId).Returns(6);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);

        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.DeleteApplication(100));

        Assert.Equal("application not found",exception.Message);
    }

    [Fact]
    public async Task DeleteApplication_WhenCandidateDoesNotOwnApplication_ThrowsForbiddenException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.UserId).Returns(6);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);

        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.DeleteApplication(2));

        Assert.Equal("Only applied candidate or admin have access to delete",exception.Message);
    }
    
    [Fact]
    public async Task DeleteApplication_WhenValidDelete_DeletesApplication()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.UserId).Returns(8);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var result=await service.CreateApplication(2,new("url"));
        bool success=await service.DeleteApplication(result.Id);
        Assert.True(success);
    }

    [Fact]
    public async Task GetApplicationById_WhenUserIsUnauthorized_ThrowsUnauthorizedException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);
        var service=new JobApplicationService(context,currentUser.Object,logger.Object);

        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.GetApplicationById(100));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task GetApplicationById_WhenApplicationDoesntExists_ThrowsNotFoundException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        var service=new JobApplicationService(context,currentUser.Object,logger.Object);

        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.GetApplicationById(100));
        Assert.Equal("Application not found",exception.Message);
    }

    [Fact]
    public async Task GetApplicationById_WhenCandidateDoesntOwnApplication_ThrowsForbiddenException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.UserId).Returns(8);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.GetApplicationById(2));
        Assert.Equal("You do not have access to this application",exception.Message);
    }

    [Fact]
    public async Task GetApplicationById_WhenRecruiterDoesntOwnJob_ThrowsForbiddenException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.UserId).Returns(1);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.GetApplicationById(3));
        Assert.Equal("You do not have access to this application",exception.Message);
    }

    [Fact]
    public async Task GetApplicationById_WhenCandidateOwnsApplication_ReturnsApplication()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.UserId).Returns(5);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var result=await service.GetApplicationById(2);
        Assert.Equal(4,result.JobId);
        Assert.Equal(5,result.CandidateId);
    }

    [Fact]
    public async Task GetApplicationById_WhenRecuiterOwnTheJob_ReturnsApplication()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.UserId).Returns(1);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var result=await service.GetApplicationById(2);
        Assert.Equal(4,result.JobId);
    }

    [Fact]
    public async Task GetApplicationById_WhenUserIsAdmin_ReturnsApplication()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.UserId).Returns(4);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Admin);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var result=await service.GetApplicationById(2);
        Assert.Equal(4,result.JobId);
    }

    [Fact]
    public async Task GetJobApplications_WhenUserIsNotUnauthorized_ThrowsUnauthorizedException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);
        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.GetJobApplications(2,pagination));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task GetJobApplications_WhenRecruiterDoesNotOwnJob_ThrowsForbiddenException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);
        currentUser.Setup(x=>x.UserId).Returns(1);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.GetJobApplications(2,pagination));
        Assert.Equal("only admin and creator of job can view job applications",exception.Message);
    }

    [Fact]
    public async Task GetJobApplications_WhenUserIsCandidate_ThrowsForbiddenException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);
        currentUser.Setup(x=>x.UserId).Returns(1);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.GetJobApplications(2,pagination));
        Assert.Equal("Only recruiter and admin can see the job applications",exception.Message);
    }

    [Fact]
    public async Task GetJobApplications_WhenRecruiterOwnJob_ReturnsApplications()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);
        currentUser.Setup(x=>x.UserId).Returns(1);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var result=await service.GetJobApplications(3,pagination);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetJobApplications_WhenUserIsAdmin_ReturnsApplications()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Admin);
        currentUser.Setup(x=>x.UserId).Returns(4);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var result=await service.GetJobApplications(3,pagination);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetMyApplications_WhenUserIsUnauthorized_ThrowsUnauthorizedException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.GetMyApplications(pagination));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Theory]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Recruiter)]
    public async Task GetMyApplications_WhenUserIsNotCandidate_ThrowsForbiddenException(UserRole Role)
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(Role);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.GetMyApplications(pagination));
        Assert.Equal("Only candidate can access this route",exception.Message);
    }

    [Fact]
    public async Task GetMyApplications_WhenCandidateHasApplications_ReturnsApplications()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.UserId).Returns(5);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var result=await service.GetMyApplications(pagination);
        Assert.NotEmpty(result.Items);
    }

    [Fact]
    public async Task GetMyApplications_WhenCandidateHasNoApplications_ReturnsApplications()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.UserId).Returns(8);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var result=await service.GetMyApplications(pagination);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task UpdateApplicationStatus_WhenApplicationDoesNotExist_ThrowsNotFoundException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.UpdateApplicationStatus(100,new(ApplicationStatus.Shortlisted)));
        Assert.Equal("Application not found",exception.Message);
    }

    [Fact]
    public async Task UpdateApplicationStatus_WhenRecruiterDoesNotOwnJob_ThrowsForbiddenException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);
        currentUser.Setup(x=>x.UserId).Returns(1);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.UpdateApplicationStatus(3,new(ApplicationStatus.Shortlisted)));
        Assert.Equal("Doesn't have access to update this application",exception.Message);
    }

    [Fact]
    public async Task UpdateApplicationStatus_WhenRecruiterOwnsJob_UpdatesJobApplication()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);
        currentUser.Setup(x=>x.UserId).Returns(8);

        var service=new JobApplicationService(context,currentUser.Object,logger.Object);

        var dto=await service.CreateApplication(3,new("url"));

        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);
        currentUser.Setup(x=>x.UserId).Returns(1);

        var result=await service.UpdateApplicationStatus(dto.Id,new(ApplicationStatus.Shortlisted));
        Assert.True(result);

        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);
        currentUser.Setup(x=>x.UserId).Returns(8);
        var updated=await service.GetApplicationById(dto.Id);
        Assert.Equal(ApplicationStatus.Shortlisted,updated.Status);

        await service.DeleteApplication(dto.Id);
    }
}