using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Moq;
using JobManagementApi.Data;
using JobManagementApi.Services;
using JobManagementApi.Exceptions;
using JobManagementApi.Dtos.JobApplication;
using JobManagementApi.Enums;
using JobManagementApi.Interfaces;

namespace JobManagementApi.Tests.Services;

public class JobApplicationServiceTests
{
    public Mock<ILogger<JobApplicationService>> logger=new();
    public Mock<ICurrentUserService> currentUser=new();

    public CreateJobApplicationDto createJobApplicationDto=new("resumeurl");

    public IConfigurationRoot configuration;
    public DapperContext context;

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
}