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
    public IConfiguration configuration;
    public DapperContext context;

    public RecruiterApplicationServiceTests()
    {
        configuration=new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        context=new DapperContext(configuration);
    }

    public Mock<ILogger<RecruiterApplicationService>> logger=new();

    [Fact]
    public async Task CreateApplication_WhenUserIsUnauthorized_ThrowsUnAuthorizedException()
    {
        
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);
        
        var service=new RecruiterApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.CreateApplication(new("reason")));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task CreateApplication_WhenUserIsNotCandidate_ThrowsForbiddenException()
    {
        
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Admin);
        
        var service=new RecruiterApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.CreateApplication(new("reason")));
        Assert.Equal("You don't have access to perform this operation",exception.Message);
    }

    [Fact]
    public async Task DeleteApplication_WhenUserIsUnauthorized_ThrowsUnAuthorizedException()
    {
        
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);
        
        var service=new RecruiterApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.DeleteApplication(1));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task DeleteApplication_WhenUserIsRecruiter_ThrowsForbiddenException()
    {
        
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);
        
        var service=new RecruiterApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.DeleteApplication(1));
        Assert.Equal("You dont have access to perform this operation",exception.Message);
    }

    [Fact]
    public async Task UpdateApplication_WhenUserIsUnauthorized_ThrowsUnAuthorizedException()
    {
        
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);
        
        var service=new RecruiterApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.UpdateApplication(1,new(RecruiterApplicationStatus.Approved)));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task UpdateApplication_WhenUserIsRecruiter_ThrowsForbiddenException()
    {
        
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);
        
        var service=new RecruiterApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.UpdateApplication(1,new(RecruiterApplicationStatus.Approved)));
        Assert.Equal("You don't have permission to perform this operation",exception.Message);
    }

    [Fact]
    public async Task UpdateApplication_WhenDataIsInvalid_ThrowsBadRequestException()
    {
        
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Admin);
        
        var service=new RecruiterApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<BadRequestException>(()=>service.UpdateApplication(1,new(RecruiterApplicationStatus.Pending)));
        Assert.Equal("Invalid application status",exception.Message);
    }

    [Fact]
    public async Task GetMyApplications_WhenUserIsUnauthorized_ThrowsUnauthorizedException()
    {
        
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new RecruiterApplicationService(context,currentUser.Object,logger.Object);

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
        
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new RecruiterApplicationService(context,currentUser.Object,logger.Object);

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
        
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new RecruiterApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.GetApplicationById(1));
    }

    [Fact]
    public async Task CreateApplication_WhenExistingApplicationInProgress_ThrowsConflictException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);
        currentUser.Setup(x=>x.UserId).Returns(8);

        var service=new RecruiterApplicationService(context,currentUser.Object,logger.Object);
        var result=await service.CreateApplication(new("test"));

        var exception=await Assert.ThrowsAsync<ConflictException>(()=>service.CreateApplication(new ("test1")));
        Assert.Equal("Already applied",exception.Message);

        await service.DeleteApplication(result.Id);
    }

    [Fact]
    public async Task CreateApplication_WhenRequestIsValid_CreatesApplication()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);
        currentUser.Setup(x=>x.UserId).Returns(8);

        var service=new RecruiterApplicationService(context,currentUser.Object,logger.Object);
        var result=await service.CreateApplication(new("test"));

        Assert.Equal("test",result.Reason);
        Assert.Equal(RecruiterApplicationStatus.Pending,result.Status);

        await service.DeleteApplication(result.Id);
    }

    [Fact]
    public async Task DeleteApplication_WhenCandidateDoesNotOwnApplication_ThrowsForbiddenException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);
        currentUser.Setup(x=>x.UserId).Returns(8);

        var service=new RecruiterApplicationService(context,currentUser.Object,logger.Object);

        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.DeleteApplication(1));
        Assert.Equal("You dont have access to perform this operation",exception.Message);
    }

    [Fact]
    public async Task DeleteApplication_WhenRequestIsValid_CreatesApplication()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);
        currentUser.Setup(x=>x.UserId).Returns(8);

        var service=new RecruiterApplicationService(context,currentUser.Object,logger.Object);
        var result=await service.CreateApplication(new("test"));
        
        bool success=await service.DeleteApplication(result.Id);
        Assert.True(success);

        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.GetApplicationById(result.Id));
        Assert.Equal("application not found",exception.Message);
    }

    [Fact]
    public async Task GetApplicationById_WhenIdIsInValid_ThrowsNotFoundException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);
        currentUser.Setup(x=>x.UserId).Returns(8);

        var service=new RecruiterApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.GetApplicationById(100));

        Assert.Equal("application not found",exception.Message);
    }

    [Fact]
    public async Task GetApplicationById_WhenIdIsValid_ThrowsNotFoundException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);
        currentUser.Setup(x=>x.UserId).Returns(5);

        var service=new RecruiterApplicationService(context,currentUser.Object,logger.Object);
        var result=await service.GetApplicationById(6);

        Assert.Equal(RecruiterApplicationStatus.Pending,result.Status);
    }

    [Fact]
    public async Task GetApplications_WhenRequestIsValid_ReturnsApplication()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);
        currentUser.Setup(x=>x.UserId).Returns(5);

        var service=new RecruiterApplicationService(context,currentUser.Object,logger.Object);

        PaginationParams pagination = new()
        {
            PageNumber=1,
            PageSize=10
        };

        var result=await service.GetMyApplications(pagination);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateApplication_WhenIdIsInValid_ThrowsNotFoundException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Admin);
        currentUser.Setup(x=>x.UserId).Returns(4);

        var service=new RecruiterApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<NotFoundException>(()=> service.UpdateApplication(100,new(RecruiterApplicationStatus.Approved)));

        Assert.Equal("application not found",exception.Message);
    }

    [Fact]
    public async Task UpdateApplication_WhenApplicationIsAlreadyProcessed_ThrowsConflictException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Admin);
        currentUser.Setup(x=>x.UserId).Returns(4);

        var service=new RecruiterApplicationService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ConflictException>(()=> service.UpdateApplication(1,new(RecruiterApplicationStatus.Approved)));

        Assert.Equal("Application has already been reviewed",exception.Message);
    }

    [Fact]
    public async Task UpdateApplication_WhenRequestIsValid_UpdatesApplication()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);
        currentUser.Setup(x=>x.UserId).Returns(8);

        var service=new RecruiterApplicationService(context,currentUser.Object,logger.Object);
        var application=await service.CreateApplication(new("reason1"));

        currentUser.Setup(x=>x.Role).Returns(UserRole.Admin);
        currentUser.Setup(x=>x.UserId).Returns(4);
        var success=await service.UpdateApplication(application.Id,new(RecruiterApplicationStatus.Rejected));
        Assert.True(success);

        var updated=await service.GetApplicationById(application.Id);
        Assert.Equal(RecruiterApplicationStatus.Rejected,updated.Status);

        await service.DeleteApplication(application.Id);
    }
}