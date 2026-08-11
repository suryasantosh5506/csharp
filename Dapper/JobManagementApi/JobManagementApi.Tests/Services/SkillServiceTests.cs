using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Moq;
using JobManagementApi.Data;
using JobManagementApi.Interfaces;
using JobManagementApi.Services;
using JobManagementApi.Exceptions;
using JobManagementApi.Enums;

namespace JobManagementApi.Tests.Services;

public class SkillServiceTests
{
    public Mock<IConfiguration> config=new();
    public Mock<DapperContext> context=null!;
    public Mock<ICurrentUserService> currentUser=new();
    public Mock<ILogger<SkillService>> logger=new();

    [Fact]
    public async Task CreateSkill_WhenUserIsUnauthorized_ThrowUnAuthorizedException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new SkillService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.CreateSkillAsync(new("skill1")));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task CreateSkill_WhenUserIsNotAdmin_ThrowsForbiddenException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);

        var service=new SkillService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.CreateSkillAsync(new("skill1")));
        Assert.Equal("You don't have access to perform this operation",exception.Message);
    }

    [Fact]
    public async Task DeleteSkill_WhenUserIsUnauthorized_ThrowUnAuthorizedException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new SkillService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.DeleteSkillAsync(1));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task DeleteSkill_WhenUserIsNotAdmin_ThrowsForbiddenException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);

        var service=new SkillService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.DeleteSkillAsync(1));
        Assert.Equal("You don't have access to perform this operation",exception.Message);
    }

    [Fact]
    public async Task UpdateSkill_WhenUserIsUnauthorized_ThrowUnAuthorizedException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new SkillService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.UpdateSkillAsync(1,new("skill1")));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task UpdateSkill_WhenUserIsNotAdmin_ThrowsForbiddenException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);

        var service=new SkillService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.UpdateSkillAsync(1,new("skill1")));
        Assert.Equal("You don't have access to perform this operation",exception.Message);
    }
}