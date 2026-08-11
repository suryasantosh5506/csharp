using Microsoft.Extensions.Logging;
using JobManagementApi.Data;
using JobManagementApi.Interfaces;
using Moq;
using JobManagementApi.Services;
using Microsoft.Extensions.Configuration;
using JobManagementApi.Exceptions;
using JobManagementApi.Enums;

namespace JobManagementApi.Tests.Services;

public class JobSkillServiceTests
{
    public Mock<DapperContext> context=null!;
    public Mock<ICurrentUserService> currentUser=new();
    public Mock<ILogger<JobSkillService>> logger=new();
    public Mock<IConfiguration> config=new();

    [Fact]
    public async Task AddSkillToJob_WhenUserIsUnauthenticated_ThrowsUnauthorizedException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);
        
        var service=new JobSkillService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.AddSkillToJob(1,1));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task AddSkillToJob_WhenUserIsCandidate_ThrowsForbiddenException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);

        var service=new JobSkillService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.AddSkillToJob(1,1));
        Assert.Equal("You don't have access to perform this operation",exception.Message);
    }

    [Fact]
    public async Task RemoveSkillFromJob_WhenUserIsUnauthenticated_ThrowsUnauthorizedException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);
        
        var service=new JobSkillService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.RemoveSkillFromJob(1,1));
        Assert.Equal("Unauthorized",exception.Message);
    }

    
    [Fact]
    public async Task RemoveSkillFromJob_WhenUserIsCandidate_ThrowsForbiddenException()
    {
        context=new(config.Object);
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);
        
        var service=new JobSkillService(context.Object,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.RemoveSkillFromJob(1,1));
        Assert.Equal("You don't have access to perform this operation",exception.Message);
    }
}