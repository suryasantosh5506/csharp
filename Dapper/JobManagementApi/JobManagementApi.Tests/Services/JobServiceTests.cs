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

namespace JobManagementApi.Tests.Services;

public class JobServiceTests
{
    public Mock<DapperContext> context=null!;
    public Mock<IConfiguration> config=new();
    public Mock<ICurrentUserService> currentUser=new();
    public Mock<ILogger<JobService>> logger=new();

    public CreateJobDto createJob=new(1,"test title","test description","test location",1000,1000,JobTypes.FullTime,0);
    public UpdateJobDto updateJob=new("updated title","updated description","test location",1000,1000,JobTypes.FullTime,0);

    [Fact]
    public async Task JobService_WhenUserIsAuthenticated_ThrowsUnauthorizedException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new JobService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.CreateJob(createJob));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task CreateJob_WhenUserIsCandidate_ThrowsForbiddenException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);

        var service=new JobService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.CreateJob(createJob));
        Assert.Equal("only recruiter and admin can create a job",exception.Message);
    }

    [Fact]
    public async Task UpdateJob_WhenUserIsUnauthenticated_ThrowsUnauthorizedException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);
        
        var service=new JobService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.UpdateJob(1,updateJob));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task UpdateJob_WhenUserIsCandidate_ThrowsForbiddenException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);
        
        var service=new JobService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.UpdateJob(1,updateJob));
        Assert.Equal("only recruiter and admin can update a job",exception.Message);
    }

    [Fact]
    public async Task UpdateJob_WhenDataIsInvalid_ThrowsBadRequestException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Admin);

        UpdateJobDto job=new("updated title","updated description","test location",10000,1000,JobTypes.FullTime,0);

        var service=new JobService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<BadRequestException>(()=>service.UpdateJob(1,job));
        Assert.Equal("Minimum salary cannot be greater than maximum salary",exception.Message);
    }

    [Fact]
    public async Task DeleteJob_WhenUserIsUnauthenticated_ThrowsUnauthorizedException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new JobService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.DeleteJob(1));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task DeleteJob_WhenUserIsCandidate_ThrowsForbiddenException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);

        var service=new JobService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.DeleteJob(1));
        Assert.Equal("only recruiter and admin can delete a job",exception.Message);
    }
}