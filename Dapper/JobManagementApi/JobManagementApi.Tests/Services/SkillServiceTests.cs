using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Moq;
using JobManagementApi.Data;
using JobManagementApi.Interfaces;
using JobManagementApi.Services;
using JobManagementApi.Exceptions;
using JobManagementApi.Enums;
using JobManagementApi.Entities;

namespace JobManagementApi.Tests.Services;

public class SkillServiceTests
{
    public IConfiguration configuration;
    public DapperContext context;
    public Mock<ICurrentUserService> currentUser=new();
    public Mock<ILogger<SkillService>> logger=new();

    public SkillServiceTests()
    {
        configuration=new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        context=new DapperContext(configuration);
    }

    [Fact]
    public async Task CreateSkill_WhenUserIsUnauthorized_ThrowUnAuthorizedException()
    {
        
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new SkillService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.CreateSkillAsync(new("skill1")));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Theory]
    [InlineData(UserRole.Candidate)]
    [InlineData(UserRole.Recruiter)]
    public async Task CreateSkill_WhenUserIsNotAdmin_ThrowsForbiddenException(UserRole Role)
    {
        
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(Role);

        var service=new SkillService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.CreateSkillAsync(new("skill1")));
        Assert.Equal("You don't have access to perform this operation",exception.Message);
    }

    [Fact]
    public async Task DeleteSkill_WhenUserIsUnauthorized_ThrowUnAuthorizedException()
    {
        
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new SkillService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.DeleteSkillAsync(1));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Theory]
    [InlineData(UserRole.Candidate)]
    [InlineData(UserRole.Recruiter)]
    public async Task DeleteSkill_WhenUserIsNotAdmin_ThrowsForbiddenException(UserRole Role)
    {
        
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(Role);

        var service=new SkillService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.DeleteSkillAsync(1));
        Assert.Equal("You don't have access to perform this operation",exception.Message);
    }

    [Fact]
    public async Task UpdateSkill_WhenUserIsUnauthorized_ThrowUnAuthorizedException()
    {
        
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new SkillService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.UpdateSkillAsync(1,new("skill1")));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Theory]
    [InlineData(UserRole.Candidate)]
    [InlineData(UserRole.Recruiter)]
    public async Task UpdateSkill_WhenUserIsNotAdmin_ThrowsForbiddenException(UserRole Role)
    {
        
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(Role);

        var service=new SkillService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.UpdateSkillAsync(1,new("skill1")));
        Assert.Equal("You don't have access to perform this operation",exception.Message);
    }

    [Fact]
    public async Task CreateSkill_WhenSkillAlreadyExist_ThrowsConflictException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Admin);
        currentUser.Setup(x=>x.UserId).Returns(4);

        var service=new SkillService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ConflictException>(()=>service.CreateSkillAsync(new("asp.net core")));
        Assert.Equal("Skill already exists",exception.Message);
    }

    [Fact]
    public async Task CreateSkill_WhenRequestIsValid_CreatesSkill()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Admin);
        currentUser.Setup(x=>x.UserId).Returns(4);

        var service=new SkillService(context,currentUser.Object,logger.Object);
        var result=await service.CreateSkillAsync(new("skill1"));
        Assert.Equal("skill1",result.Name);

        await service.DeleteSkillAsync(result.Id);
    }

    [Fact]
    public async Task DeleteSkill_WhenIdIsInValid_ThrowsNotFoundException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Admin);
        currentUser.Setup(x=>x.UserId).Returns(4);

        var service=new SkillService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.DeleteSkillAsync(100));
        Assert.Equal("skill not found",exception.Message);
    }

    [Fact]
    public async Task DeleteSkill_WhenRequestIsValid_DeletesSkill()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Admin);
        currentUser.Setup(x=>x.UserId).Returns(4);

        var service=new SkillService(context,currentUser.Object,logger.Object);
        var skill=await service.CreateSkillAsync(new("skill1"));

        bool success=await service.DeleteSkillAsync(skill.Id);
        Assert.True(success);

        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.GetSkillAsync(skill.Id));
        Assert.Equal("skill not found",exception.Message);
    }

    [Fact]
    public async Task UpdateSkill_WhenIdIsInValid_ThrowsNotFoundException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Admin);
        currentUser.Setup(x=>x.UserId).Returns(4);

        var service=new SkillService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.UpdateSkillAsync(100,new("updated name")));
        Assert.Equal("skill not found",exception.Message);
    }

    [Fact]
    public async Task UpdateSkill_WhenRequestIsValid_DeletesSkill()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Admin);
        currentUser.Setup(x=>x.UserId).Returns(4);

        var service=new SkillService(context,currentUser.Object,logger.Object);
        var skill=await service.CreateSkillAsync(new("skill1"));

        bool success=await service.UpdateSkillAsync(skill.Id,new("updated name"));
        var updated=await service.GetSkillAsync(skill.Id);

        Assert.True(success);
        Assert.Equal("updated name",updated.Name);

        await service.DeleteSkillAsync(skill.Id);

        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.GetSkillAsync(skill.Id));
        Assert.Equal("skill not found",exception.Message);
    }

    [Fact]
    public async Task GetSkill_WhenIdIsInValid_ThrowsNotFoundException()
    {
        var service=new SkillService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.GetSkillAsync(100));
        Assert.Equal("skill not found",exception.Message);
    }

    [Fact]
    public async Task GetSkill_WhenIdIsValid_ReturnsSkill()
    {
        var service=new SkillService(context,currentUser.Object,logger.Object);
        var skill=await service.GetSkillAsync(1);
        Assert.Equal("c#",skill.Name);
    }

    [Fact]
    public async Task GetAllSkills_WhenRequestIsValid_ReturnsSkills()
    {
        var service=new SkillService(context,currentUser.Object,logger.Object);
        var skills=await service.GetAllSkillsAsync(new(){PageNumber=1,PageSize=10});
        Assert.NotNull(skills);
    }
}