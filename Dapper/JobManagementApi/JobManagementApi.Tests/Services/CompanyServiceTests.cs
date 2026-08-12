using Microsoft.Extensions.Logging;
using JobManagementApi.Data;
using JobManagementApi.Interfaces;
using Moq;
using JobManagementApi.Services;
using JobManagementApi.Dtos.Company;
using JobManagementApi.Exceptions;
using Microsoft.Extensions.Configuration;
using JobManagementApi.Enums;
using Microsoft.VisualBasic;
using Dapper;
using JobManagementApi.RequestHelpers.Searching;
using JobManagementApi.Entities;
namespace JobManagementApi.Tests.Services;

public class CompanyServiceTests
{
    private Mock<ICurrentUserService> _currentUser=new();
    private Mock<ILogger<CompanyService>> _logger=new();
    private Mock<IConfiguration> _config=new();

    private IConfigurationRoot configuration;
    private DapperContext _context;

    public CompanyServiceTests()
    {
        configuration=new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        _context=new DapperContext(configuration);
    }


    private CreateCompanyDto _createDto= new("Test Company","Test Description","Hyderabad","https://test.com");
    private UpdateCompanyDto _updateDto=new("Updated Company","Updated Description","Hyderabad","https://updated.com");


    [Fact]
    public async Task CreateCompany_WhenUserIsUnauthenticated_ThrowsUnauthorizedException()
    {
        
        _currentUser.Setup(x=>x.IsAuthenticated).Returns(false);
        
        var service=new CompanyService(_context,_currentUser.Object,_logger.Object);
        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.CreateCompany(_createDto));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task CreateCompany_WhenUserIsCandidate_ThrowsForbiddenException()
    {
        _currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        _currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);

        var service=new CompanyService(_context,_currentUser.Object,_logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.CreateCompany(_createDto));
        Assert.Equal("Only admin and recruiter can create a company",exception.Message);
    }

    [Fact]
    public async Task DeleteCompany_WhenUserIsUnauthenticated_ThrowsUnauthorizedException()
    {
        _currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new CompanyService(_context,_currentUser.Object,_logger.Object);

        var exception=await Assert.ThrowsAnyAsync<UnauthorizedException>(()=>service.DeleteCompany(1));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task DeleteCompany_WhenUserIsCandidate_ThrowsForbiddenException()
    {
        _currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        _currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);

        var service=new CompanyService(_context,_currentUser.Object,_logger.Object);

        var exception=await Assert.ThrowsAnyAsync<ForbiddenException>(()=>service.DeleteCompany(1));
        Assert.Equal("Only admin and recruiter can delete a company",exception.Message);
    }

    [Fact]
    public async Task UpdateCompany_WhenUserIsUnauthenticated_ThrowsUnauthorizedException()
    {
        _currentUser.Setup(x=>x.IsAuthenticated).Returns(false);

        var service=new CompanyService(_context,_currentUser.Object,_logger.Object);

        var exception=await Assert.ThrowsAsync<UnauthorizedException>(()=>service.UpdateCompany(1,_updateDto));
        Assert.Equal("Unauthorized",exception.Message);
    }

    [Fact]
    public async Task UpdateCompany_WhenUserIsCandidate_ThrowsForbiddenException()
    {
        _currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        _currentUser.Setup(x=>x.Role).Returns(UserRole.Candidate);
        var service=new CompanyService(_context,_currentUser.Object,_logger.Object);

        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.UpdateCompany(1,_updateDto));
        Assert.Equal("Only admin and recruiter can update a company",exception.Message);
    }

    [Theory]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Recruiter)]
    public async Task CreateCompany_WhenCompanyAlreadyExists_ThrowsConflictException(UserRole Role)
    {
        _currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        _currentUser.Setup(x=>x.Role).Returns(Role);

        var dto=new CreateCompanyDto(
            "technova solutions",
            "Test Description",
            "Hyderabad",
            "https://test.com"
        );

        var service=new CompanyService(_context,_currentUser.Object,_logger.Object);
        var exception=await Assert.ThrowsAsync<ConflictException>(()=>service.CreateCompany(dto));
        Assert.Equal("Company with specified name already exists",exception.Message);
    }

    [Theory]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Recruiter)]
    public async Task CreateCompany_WhenValidRequest_CreatesCompanySuccessfully(UserRole Role)
    {
        _currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        _currentUser.Setup(x=>x.Role).Returns(Role);
        _currentUser.Setup(x=>x.UserId).Returns(4);

        var dto=new CreateCompanyDto(
            "Unit Test Company12345",
            "Test company created during Unit testing",
            "Hyderabad",
            "https://unittest.com"
        );

        var service=new CompanyService(_context,_currentUser.Object,_logger.Object);
        var result=await service.CreateCompany(dto);
        Assert.NotNull(result);
        Assert.NotEqual(0,result.Id);
        Assert.Equal(dto.Name.ToLower(),result.Name);
        await service.DeleteCompany(result.Id);
    }

    [Theory]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Recruiter)]
    public async Task DeleteCompany_WhenCompanyDoesNotExist_ThrowsNotFoundException(UserRole Role)
    {
        _currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        _currentUser.Setup(x=>x.Role).Returns(Role);

        var service=new CompanyService(_context,_currentUser.Object,_logger.Object);
        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.DeleteCompany(100));
        Assert.Equal("Company not found",exception.Message);
    }

    [Theory]
    [InlineData(UserRole.Recruiter)]
    public async Task DeleteCompany_WhenUserDoesNotOwnCompany_ThrowsForbiddenException(UserRole Role)
    {
        _currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        _currentUser.Setup(x=>x.Role).Returns(Role);
        _currentUser.Setup(x=>x.UserId).Returns(7);

        var service=new CompanyService(_context,_currentUser.Object,_logger.Object);
        var exception=await Assert.ThrowsAsync<ForbiddenException>(()=>service.DeleteCompany(6));
        Assert.Equal("Only admin and recruiter can delete a company",exception.Message);
    }


    [Theory]
    [InlineData(UserRole.Recruiter)]
    public async Task DeleteCompany_WhenValidRequest_DeletesCompanySuccessfully(UserRole Role)
    {
        _currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        _currentUser.Setup(x=>x.Role).Returns(Role);
        _currentUser.Setup(x=>x.UserId).Returns(4);

        var dto=new CreateCompanyDto(
            "Unit Test Company12345",
            "Test company created during Unit testing",
            "Hyderabad",
            "https://unittest.com"
        );

        var service=new CompanyService(_context,_currentUser.Object,_logger.Object);
        var company=await service.CreateCompany(dto);

        var result=await service.DeleteCompany(company.Id);
        Assert.True(result);
    }

    [Fact]
    public async Task GetCompanies_ReturnsCompanies()
    {
        var service=new CompanyService(_context,_currentUser.Object,_logger.Object);
        CompanyParams companyParams = new()
        {
            Location="",
            PageNumber=1,
            PageSize=10,
            Search=""
        };
        var result=await service.GetCompanies(companyParams);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetCompanyById_WhenCompanyExists_ReturnsCompany()
    {
        var service=new CompanyService(_context,_currentUser.Object,_logger.Object);
        var result=await service.GetCompanyById(1);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetCompanyById_WhenCompanyDoesNotExist_ThrowsNotFoundException()
    {
        var service=new CompanyService(_context,_currentUser.Object,_logger.Object);
        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.GetCompanyById(100));
        Assert.Equal("Company not found",exception.Message);
    }

    [Theory]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Recruiter)]
    public async Task UpdateCompany_WhenCompanyDoesNotExist_ThrowsNotFoundException(UserRole Role)
    {
        _currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        _currentUser.Setup(x=>x.Role).Returns(Role);
        _currentUser.Setup(x=>x.UserId).Returns(4);

        var updatedto=new UpdateCompanyDto(
            "Unit Test Company 54321",
            "Test company created during Unit testing",
            "Hyderabad",
            "https://unittest.com"
        );

        var service=new CompanyService(_context,_currentUser.Object,_logger.Object);
        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.UpdateCompany(100,updatedto));
        Assert.Equal("Company Not found",exception.Message);
    }

    [Fact]
    public async Task UpdateCompany_WhenUserDoesNotOwnCompany_ThrowsForbiddenException()
    {
        _currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        _currentUser.Setup(x=>x.Role).Returns(UserRole.Recruiter);
        _currentUser.Setup(x=>x.UserId).Returns(1);

        var updatedto=new UpdateCompanyDto(
            "Unit Test Company 54321",
            "Test company created during Unit testing",
            "Hyderabad",
            "https://unittest.com"
        );
        var service=new CompanyService(_context,_currentUser.Object,_logger.Object);
        var exception=await Assert.ThrowsAsync<NotFoundException>(()=>service.UpdateCompany(10,updatedto));
        Assert.Equal("You do not have permission to update this company",exception.Message);
    }

    [Theory]
    [InlineData(UserRole.Admin,4)]
    [InlineData(UserRole.Recruiter,1)]
    public async Task UpdateCompany_WhenValidRequest_UpdatesCompanySuccessfully(UserRole Role,int UserId)
    {
        _currentUser.Setup(x=>x.IsAuthenticated).Returns(true);
        _currentUser.Setup(x=>x.Role).Returns(Role);
        _currentUser.Setup(x=>x.UserId).Returns(UserId);

        var dto=new CreateCompanyDto(
            "Unit Test Company12345",
            "Test company created during Unit testing",
            "Hyderabad",
            "https://unittest.com"
        );

        var service=new CompanyService(_context,_currentUser.Object,_logger.Object);
        var result=await service.CreateCompany(dto);

        var updatedto=new UpdateCompanyDto(
            "Unit Test Company 54321",
            "Test company created during Unit testing",
            "Hyderabad",
            "https://unittest.com"
        );
        bool success=await service.UpdateCompany(result.Id,updatedto);
        Assert.True(success);
        var res=await service.GetCompanyById(result.Id);
        Assert.Equal(updatedto.Name.Trim().ToLower(),res.Name);

        await service.DeleteCompany(result.Id);
    }
}