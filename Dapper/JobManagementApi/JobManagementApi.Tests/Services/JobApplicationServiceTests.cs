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
    public Mock<IConfiguration> config=new();
    public Mock<DapperContext> context=null!;
    public Mock<ILogger<JobApplicationService>> logger=new();
    public Mock<ICurrentUserService> currentUser=new();

    public CreateJobApplicationDto createJobApplicationDto=new("resumeurl");

    [Fact]
    public async Task CreateApplication_WhenUserIsUnAuthenticated_ThrowsUnauthorizedException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new JobApplicationService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.CreateApplication(1,createJobApplicationDto));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task CreateApplication_WhenUserIsCandidate_ThrowsForbiddenException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);

        var service=new JobApplicationService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.CreateApplication(1,createJobApplicationDto));
        Assert.Equal("Only candidate can apply",exception.Message);
    }

    [Fact]
    public async Task DeleteApplication_WhenUserIsUnAuthenticated_ThrowsUnauthorizedException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new JobApplicationService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.DeleteApplication(1));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task DeleteApplication_WhenUserIsRecruiter_ThrowsForbiddenException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);

        var service=new JobApplicationService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.DeleteApplication(1));
        Assert.Equal("Only candidate and admin can delete an application",exception.Message);
    }

    [Fact]
    public async Task UpdateApplicationStatus_WhenUserIsUnAuthenticated_ThrowsUnauthorizedException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new JobApplicationService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.UpdateApplicationStatus(1,new (ApplicationStatus.Applied)));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task UpdateApplicationStatus_WhenUserIsCandidate_ThrowsForbiddenException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);

        var service=new JobApplicationService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.UpdateApplicationStatus(1,new (ApplicationStatus.Applied)));
        Assert.Equal("Only recruiter and admin can delete an application",exception.Message);
    }
}