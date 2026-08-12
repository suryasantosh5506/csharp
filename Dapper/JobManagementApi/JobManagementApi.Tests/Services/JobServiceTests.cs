using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Castle.Core.Logging;
using JobManagementApi.Data;
using JobManagementApi.Services;
using Moq;
using JobManagementApi.Dtos.Jobs;
using JobManagementApi.Enums;
using JobManagementApi.Exceptions;
using JobManagementApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobManagementApi.Tests.Services;

public class JobServiceTests
{
    public DapperContext context;
    public IConfigurationRoot configuration;
    public Mock<ICurrentUserService> currentUser=new();
    public Mock<ILogger<JobService>> logger=new();

    public CreateJobDto createJob=new(1,"test title","test description","test location",1000,1000,JobTypes.FullTime,0);
    public UpdateJobDto updateJob=new("updated title","updated description","test location",1000,1000,JobTypes.FullTime,0);

    public JobServiceTests()
    {
        configuration=new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        context=new DapperContext(configuration);
    }

    [Fact]
    public async Task CreateJob_WhenUserIsAuthenticated_ThrowsUnauthorizedException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new JobService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.CreateJob(createJob));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task CreateJob_WhenUserIsCandidate_ThrowsForbiddenException()
    {  
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);

        var service=new JobService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.CreateJob(createJob));
        Assert.Equal("only recruiter and admin can create a job",exception.Message);
    }

    [Fact]
    public async Task UpdateJob_WhenUserIsUnauthenticated_ThrowsUnauthorizedException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);
        
        var service=new JobService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.UpdateJob(1,updateJob));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task UpdateJob_WhenUserIsCandidate_ThrowsForbiddenException()
    {   
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);
        
        var service=new JobService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.UpdateJob(1,updateJob));
        Assert.Equal("only recruiter and admin can update a job",exception.Message);
    }

    [Fact]
    public async Task UpdateJob_WhenDataIsInvalid_ThrowsBadRequestException()
    {   
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Admin);

        UpdateJobDto job=new("updated title","updated description","test location",10000,1000,JobTypes.FullTime,0);

        var service=new JobService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<BadRequestException>(()=>service.UpdateJob(1,job));
        Assert.Equal("Minimum salary cannot be greater than maximum salary",exception.Message);
    }

    [Fact]
    public async Task DeleteJob_WhenUserIsUnauthenticated_ThrowsUnauthorizedException()
    {   
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new JobService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.DeleteJob(1));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task DeleteJob_WhenUserIsCandidate_ThrowsForbiddenException()
    {   
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);

        var service=new JobService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.DeleteJob(1));
        Assert.Equal("only recruiter and admin can delete a job",exception.Message);
    }

    [Fact]
    public async Task CreateJob_WhenCompanyNotExists_ThrowsNotFoundException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);
        var service=new JobService(context,currentUser.Object,logger.Object);
        CreateJobDto dto=new(100,"test title","test description","test location",1000,1000,JobTypes.FullTime,0);
        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.CreateJob(dto));
        Assert.Equal("company not found",exception.Message);
    }

    [Fact]
    public async Task CreateJob_WhenRecruiterNotOwnsCompany_ThrowsForbiddenException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);
        currentUser.Setup(x=>x.UserId).Returns(1);
        var service=new JobService(context,currentUser.Object,logger.Object);
        CreateJobDto dto=new(1,"test title","test description","test location",1000,1000,JobTypes.FullTime,0);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.CreateJob(dto));
        Assert.Equal("You do not have permission to create a job for this company",exception.Message);
    }

    [Fact]
    public async Task CreateJob_WhenRecruiterOwnsCompany_ThrowsForbiddenException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);
        currentUser.Setup(x=>x.UserId).Returns(1);
        var service=new JobService(context,currentUser.Object,logger.Object);
        CreateJobDto dto=new(2,"test title","test description","test location",1000,1000,JobTypes.FullTime,0);
        var result=await service.CreateJob(dto);
        Assert.Equal(dto.CompanyId,result.CompanyId);
        Assert.Equal(dto.Title.Trim().ToLower(),result.Title.Trim().ToLower());
        await service.DeleteJob(result.Id);
    }

    [Fact]
    public async Task GetJobById_WhenJobDoesNotExist_ThrowsNotFoundException()
    {
        var service=new JobService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.GetJobById(100));
        Assert.Equal("Job not found",exception.Message);
    }

    [Fact]
    public async Task GetJobById_WhenJobExist_ThrowsNotFoundException()
    {
        var service=new JobService(context,currentUser.Object,logger.Object);
        var result=await service.GetJobById(5);
        Assert.Equal(5,result.Id);
        Assert.Equal(1,result.CompanyId);
        Assert.Equal(1,result.RecruiterId);
    }

    [Fact]
    public async Task UpdateJob_WhenJobDoesNotExist_ThrowsNotFoundException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);
        var service=new JobService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.UpdateJob(100,updateJob));
        Assert.Equal("Job not found",exception.Message);
    }

    [Fact]
    public async Task UpdateJob_WhenRecruiterDoesNotOwnCompany_ThrowsNotFoundException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);
        currentUser.Setup(x=>x.UserId).Returns(1);
        var service=new JobService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.UpdateJob(1,updateJob));
        Assert.Equal("Job not found",exception.Message);
    }

    [Fact]
    public async Task UpdateJob_WhenValidRequest_UpdatesJobSuccessfully()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);
        currentUser.Setup(x=>x.UserId).Returns(1);

        var service=new JobService(context,currentUser.Object,logger.Object);
        CreateJobDto createnewJob=new(2,"test title","test description","test location",1000,1000,JobTypes.FullTime,0);

        var created=await service.CreateJob(createnewJob);

    //     =new(1,"test title","test description","test location",1000,1000,JobTypes.FullTime,0);
    // public UpdateJobDto updateJob=new("updated title","updated description","test location",1000,1000,JobTypes.FullTime,0);

        var success=await service.UpdateJob(created.Id,updateJob);
        Assert.True(success);
        var updated=await service.GetJobById(created.Id);
        Assert.Equal(updateJob.Title,updated.Title);
        
        await service.DeleteJob(created.Id);
    }



    [Fact]
    public async Task DeleteJob_WhenJobDoesNotExist_ThrowsNotFoundException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);
        var service=new JobService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.DeleteJob(100));
        Assert.Equal("Job not found",exception.Message);
    }

    [Fact]
    public async Task DeleteJob_WhenRecruiterDoesNotOwnCompany_ThrowsNotFoundException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);
        currentUser.Setup(x=>x.UserId).Returns(1);
        var service=new JobService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.DeleteJob(1));
        Assert.Equal("Job not found",exception.Message);
    }

    [Fact]
    public async Task DeleteJob_WhenValidRequest_UpdatesJobSuccessfully()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);
        currentUser.Setup(x=>x.UserId).Returns(1);

        var service=new JobService(context,currentUser.Object,logger.Object);
        CreateJobDto createnewJob=new(2,"test title","test description","test location",1000,1000,JobTypes.FullTime,0);

        var created=await service.CreateJob(createnewJob);

        bool success=await service.DeleteJob(created.Id);
        Assert.True(success);
    }
}