using JobManagementApi.Data;
using JobManagementApi.Entities;
using JobManagementApi.Enums;
using JobManagementApi.Exceptions;
using JobManagementApi.Interfaces;
using JobManagementApi.RequestHelpers.Pagination;
using JobManagementApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace JobManagementApi.Tests.Services;

public class RecruiterApplicationServiceTests
{
    public Mock<ICurrentUserService> currentUser=new();
    public Mock<IConfiguration> config=new();
    public Mock<DapperContext> context=null!;
    public Mock<ILogger<RecruiterApplicationService>> logger=new();

    [Fact]
    public async Task CreateApplication_WhenUserIsUnauthorized_ThrowsUnAuthorizedException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);
        
        var service=new RecruiterApplicationService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.CreateApplication(new("reason")));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task CreateApplication_WhenUserIsNotCandidate_ThrowsForbiddenException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Admin);
        
        var service=new RecruiterApplicationService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.CreateApplication(new("reason")));
        Assert.Equal("You don't have access to perform this operation",exception.Message);
    }

    [Fact]
    public async Task DeleteApplication_WhenUserIsUnauthorized_ThrowsUnAuthorizedException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);
        
        var service=new RecruiterApplicationService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.DeleteApplication(1));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task DeleteApplication_WhenUserIsRecruiter_ThrowsForbiddenException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);
        
        var service=new RecruiterApplicationService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.DeleteApplication(1));
        Assert.Equal("You dont have access to perform this operation",exception.Message);
    }

    [Fact]
    public async Task UpdateApplication_WhenUserIsUnauthorized_ThrowsUnAuthorizedException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);
        
        var service=new RecruiterApplicationService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.UpdateApplication(1,new(RecruiterApplicationStatus.Approved)));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task UpdateApplication_WhenUserIsRecruiter_ThrowsForbiddenException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);
        
        var service=new RecruiterApplicationService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.UpdateApplication(1,new(RecruiterApplicationStatus.Approved)));
        Assert.Equal("You don't have permission to perform this operation",exception.Message);
    }

    [Fact]
    public async Task UpdateApplication_WhenDataIsInvalid_ThrowsBadRequestException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Admin);
        
        var service=new RecruiterApplicationService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<BadRequestException>(()=>service.UpdateApplication(1,new(RecruiterApplicationStatus.Pending)));
        Assert.Equal("Invalid application status",exception.Message);
    }

    [Fact]
    public async Task GetMyApplications_WhenUserIsUnauthorized_ThrowsUnauthorizedException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new RecruiterApplicationService(context.Object,currentUser.Object,logger.Object);

        PaginationParams paginationParams= new()
        {
            PageSize=10,
            PageNumber=1
        };

        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.GetMyApplications(paginationParams));
    }

    [Fact]
    public async Task GetApplications_WhenUserIsUnauthorized_ThrowsUnauthorizedException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new RecruiterApplicationService(context.Object,currentUser.Object,logger.Object);

        PaginationParams paginationParams= new()
        {
            PageSize=10,
            PageNumber=1
        };

        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.GetApplications(paginationParams));
    }

    [Fact]
    public async Task GetApplicationById_WhenUserIsUnauthorized_ThrowsUnauthorizedException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new RecruiterApplicationService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.GetApplicationById(1));
    }
}