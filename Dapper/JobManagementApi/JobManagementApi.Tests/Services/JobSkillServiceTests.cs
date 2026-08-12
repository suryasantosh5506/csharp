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
    public DapperContext context;
    public Mock<ICurrentUserService> currentUser=new();
    public Mock<ILogger<JobSkillService>> logger=new();
    public IConfiguration configuration;

     public JobSkillServiceTests()
    {
        configuration=new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        context=new DapperContext(configuration);
    }


    [Fact]
    public async Task AddSkillToJob_WhenUserIsUnauthenticated_ThrowsUnauthorizedException()
    {
        
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);
        
        var service=new JobSkillService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.AddSkillToJob(1,1));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task AddSkillToJob_WhenUserIsCandidate_ThrowsForbiddenException()
    {
        
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);

        var service=new JobSkillService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.AddSkillToJob(1,1));
        Assert.Equal("You don't have access to perform this operation",exception.Message);
    }

    [Fact]
    public async Task RemoveSkillFromJob_WhenUserIsUnauthenticated_ThrowsUnauthorizedException()
    {
        
        currentUser.Setup(x=>x.IsAuthenticated).Returns(false);
        
        var service=new JobSkillService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.RemoveSkillFromJob(1,1));
        Assert.Equal("Unauthorized",exception.Message);
    }

    
    [Fact]
    public async Task RemoveSkillFromJob_WhenUserIsCandidate_ThrowsForbiddenException()
    {
        
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);
        
        var service=new JobSkillService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.RemoveSkillFromJob(1,1));
        Assert.Equal("You don't have access to perform this operation",exception.Message);
    }

    [Fact]
    public async Task AddSkillToJob_WhenJobIsInvalid_ThrowsNotFoundException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);
        var service=new JobSkillService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.AddSkillToJob(100,100));
        Assert.Equal("Job Not Found",exception.Message);
    }

    [Fact]
    public async Task AddSkillToJob_WhenSkillIsInvalid_ThrowsNotFoundException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);
        var service=new JobSkillService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.AddSkillToJob(2,100));
        Assert.Equal("Skill Not Found",exception.Message);
    }

    [Fact]
    public async Task AddSkillToJob_WhenSkillIsAlreadyAttached_ThrowsConflictException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Admin);
        currentUser.Setup(x=>x.UserId).Returns(4);
        var service=new JobSkillService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ConflictException>(()=>service.AddSkillToJob(2,1));
        Assert.Equal("Skill already added",exception.Message);
    }

    [Fact]
    public async Task AddSkillToJob_WhenRecruiterDoesNotOwnJob_ThrowsForbiddenException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);
        currentUser.Setup(x=>x.UserId).Returns(1);
        var service=new JobSkillService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.AddSkillToJob(2,1));
        Assert.Equal("You do not have access to this job",exception.Message);
    }

    [Fact]
    public async Task AddSkillToJob_WhenRequestIsValid_SkillAddedToJob()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Admin);
        currentUser.Setup(x=>x.UserId).Returns(4);
        var service=new JobSkillService(context,currentUser.Object,logger.Object);

        var success=await service.AddSkillToJob(2,3);
        Assert.True(success);

        await service.RemoveSkillFromJob(2,3);
    }

    [Fact]
    public async Task RemoveSkillFromJob_WhenJobIsInvalid_ThrowsNotFoundException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);
        var service=new JobSkillService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.RemoveSkillFromJob(100,100));
        Assert.Equal("Job Not Found",exception.Message);
    }

    [Fact]
    public async Task RemoveSkillFromJob_WhenSkillIsInvalid_ThrowsNotFoundException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);
        var service=new JobSkillService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.RemoveSkillFromJob(2,100));
        Assert.Equal("Skill Not Found",exception.Message);
    }

    [Fact]
    public async Task RemoveSkillFromJob_WhenSkillIsNotAttached_ThrowsConflictException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Admin);
        currentUser.Setup(x=>x.UserId).Returns(4);
        var service=new JobSkillService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ConflictException>(()=>service.RemoveSkillFromJob(2,3));
        Assert.Equal("skill was not associated with this job",exception.Message);
    }

    [Fact]
    public async Task RemoveSkillFromJob_WhenRequestIsValid_SkillRemovedFromJob()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Admin);
        currentUser.Setup(x=>x.UserId).Returns(4);
        var service=new JobSkillService(context,currentUser.Object,logger.Object);

        await service.AddSkillToJob(2,3);

        var success=await service.RemoveSkillFromJob(2,3);
        Assert.True(success);
    }

    [Fact]
    public async Task RemoveSkillFromJob_WhenRecruiterDoesNotOwnJob_ThrowsForbiddenException()
    {
        currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);
        currentUser.Setup(x=>x.UserId).Returns(1);
        var service=new JobSkillService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.RemoveSkillFromJob(2,1));
        Assert.Equal("You do not have access to this job",exception.Message);
    }

    [Fact]
    public async Task GetJobSkills_WhenJobIsInValid_ThrowsNotFoundException()
    {
        var service=new JobSkillService(context,currentUser.Object,logger.Object);
        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.GetJobSkills(100));
        Assert.Equal("Job Not Found",exception.Message);
    }

    [Fact]
    public async Task GetJobSkills_WhenJobIsValid_ThrowsNotFoundException()
    {
        var service=new JobSkillService(context,currentUser.Object,logger.Object);
        var result=service.GetJobSkills(2);
        Assert.NotNull(result);
    }
}