using System.Data;
using System.Text;
using Dapper;
using JobManagementApi.Data;
using JobManagementApi.Dtos.Company;
using JobManagementApi.Entities;
using JobManagementApi.Enums;
using JobManagementApi.Exceptions;
using JobManagementApi.Extensions;
using JobManagementApi.Interfaces;
using JobManagementApi.RequestHelpers.Pagination;
using JobManagementApi.RequestHelpers.Searching;

namespace JobManagementApi.Services;

public class CompanyService(DapperContext context, ICurrentUserService currentUser,ILogger<CompanyService>logger) : ICompanyService
{
    public async Task<CompanyDto> CreateCompany(CreateCompanyDto dto)
    {
        if (!currentUser.IsAuthenticated)
        {
            logger.LogWarning("Unauthorized access:Some tried to register a company without proper authentication");
            throw new UnauthorizedException("Unauthorized");
        }
        if(currentUser.Role!=UserRole.Admin && currentUser.Role != UserRole.Recruiter)
        {
            logger.LogWarning($"User {currentUser.UserId} tried to create a company without permission");
            throw new ForbiddenException("Only admin and recruiter can create a company");
        }
        using var connection=context.GetConnection();
        string query="Select * from company where name=@name";
        Company? existingCompany=await connection.QueryFirstOrDefaultAsync<Company?>(query,new{name=dto.Name.Trim().ToLower()});
        if(existingCompany is not null)
        {
            logger.LogWarning($"User {currentUser.UserId} tries to register a company with already registered name");
            throw new ConflictException("Company with specified name already exists");
        }
        var parameters = new
        {
            p_UserId=currentUser.UserId,
            p_Name=dto.Name.Trim().ToLower(),
            p_Description=string.IsNullOrEmpty(dto.Description)?string.Empty:dto.Description,
            p_Location=dto.Location.Trim().ToLower(),
            p_Website=dto.Website.Trim().ToLower()
        };
        int rowsaffected=await connection.ExecuteAsync("InsertCompany",parameters,commandType:CommandType.StoredProcedure);
        if (rowsaffected == 0)
        {
            logger.LogCritical("Company registration failed:Database responded with 0 rows affected");
            throw new Exception("Internal server error");
        }
        var company=await connection.QueryFirstAsync<Company>("select * from company where name=@name",new{name=dto.Name.Trim().ToLower()});
        logger.LogInformation($"company {company.Id} registration successful");
        logger.LogInformation($"User {currentUser.UserId} created Company {company.Id}");
        return company.ToDto();
    }

    public async Task<bool> DeleteCompany(int id)
    {
        if (!currentUser.IsAuthenticated)
        {
            logger.LogWarning("Unauthorized access:Some tried to delete the company without proper authentication");
            throw new UnauthorizedException("Unauthorized");
        }
        if(currentUser.Role!=UserRole.Admin && currentUser.Role != UserRole.Recruiter)
        {
            logger.LogWarning($"User {currentUser.UserId} tries to delete a company without proper permissions");
            throw new ForbiddenException("Only admin and recruiter can delete a company");
        }
        using var connection=context.GetConnection();
        var company=await connection.QueryFirstOrDefaultAsync<Company?>("GetCompanyById",new{p_Id=id},commandType:CommandType.StoredProcedure);
        if(company is null)
        {
            logger.LogInformation($"user {currentUser.UserId} tries to delete company that doesn't exist");
            throw new NotFoundException("Company not found");
        }
        if(company.UserId!=currentUser.UserId && currentUser.Role != UserRole.Admin)
        {
            logger.LogWarning($"User {currentUser.UserId} tried to delete a company without permission");
            throw new ForbiddenException("Only admin and recruiter can delete a company");
        }
        int rowsaffected=await connection.ExecuteAsync("DeleteCompanyById",new{p_Id=id},commandType:CommandType.StoredProcedure);
        if (rowsaffected == 0)
        {
            logger.LogCritical("Company Deletion failed:Database responded with 0 rows affected");
            throw new Exception("Internal server error");
        }
        logger.LogInformation("company deleted successfully");
        return true;
    }

    public async Task<PagedList<CompanyDto>> GetCompanies(CompanyParams companyParams)
    {
        using var connection=context.GetConnection();
        StringBuilder query=new StringBuilder();
        StringBuilder countQuery=new();
        countQuery.Append("select count(*) from company");
        query.Append("Select * from company ");
        
        StringBuilder conditions=new();


        if (!string.IsNullOrWhiteSpace(companyParams.Search))
        {
            conditions.Append(" (name like @search or description like @search) ");
        }

        if (!string.IsNullOrWhiteSpace(companyParams.Location))
        {
            if(conditions.Length>0) conditions.Append(" and ");
            conditions.Append(" location like @location ");
        }

        if (conditions.Length > 0)
        {
            query.Append(" where ");
            countQuery.Append(" where ");
            query.Append(conditions);
            countQuery.Append(conditions);
        }

        query.Append(" order by id asc limit @limit offset @offset");

        var parameters=new{
            limit=companyParams.PageSize,
            offset=(companyParams.PageNumber-1)*companyParams.PageSize,
            search=$"%{companyParams.Search?.ToLower().Trim()}%",
            location=$"%{companyParams.Location?.ToLower().Trim()}%"
        };
        var companies=await connection.QueryAsync<Company>(query.ToString(),parameters);
        int totalCount=await connection.ExecuteScalarAsync<int>(countQuery.ToString(),parameters);
        return PagedList<CompanyDto>.ToPagedList(companies.Select(x=>x.ToDto()),companyParams.PageNumber,totalCount,companyParams.PageSize);
    }

    public async Task<CompanyDto> GetCompanyById(int id)
    {
        using var connection=context.GetConnection();
        var company=await connection.QueryFirstOrDefaultAsync<Company?>("GetCompanyById",new{p_Id=id},commandType:CommandType.StoredProcedure);
        if(company is null)
        {
            if(currentUser.IsAuthenticated) logger.LogInformation($"user {currentUser.UserId} tried to access company that is not registered");
            else logger.LogInformation($"Anonymous user tried to access company that is not registered");
            throw new NotFoundException("Company not found");
        }
        return company.ToDto();
    }
    public async Task<bool> UpdateCompany(int id, UpdateCompanyDto dto)
    {
        if (!currentUser.IsAuthenticated)
        {
            logger.LogWarning("Unauthorized access:Some tried to register a company without proper authentication");
            throw new UnauthorizedException("Unauthorized");
        }
        if(currentUser.Role!=UserRole.Admin && currentUser.Role != UserRole.Recruiter)
        {
            logger.LogWarning($"User {currentUser.UserId} tries to update a company without proper permission");
            throw new ForbiddenException("Only admin and recruiter can update a company");
        }
        using var connection=context.GetConnection();
        
        Company? existingCompany=await connection.QueryFirstOrDefaultAsync<Company?>("GetCompanyById",new{p_Id=id},commandType:CommandType.StoredProcedure);

        if(existingCompany is null)
        {
            logger.LogInformation($"user {currentUser.UserId} tried to update the company that is not registered");
            throw new NotFoundException("Company Not found");
        }

        if (existingCompany.UserId != currentUser.UserId && currentUser.Role != UserRole.Admin)
        {
            logger.LogWarning($"User {currentUser.UserId} tries to update a company without proper permission");
            throw new ForbiddenException("You do not have permission to update this company");
        }

        var parameters = new
        {
            p_Id=id,
            p_Name=dto.Name.Trim().ToLower(),
            p_Description=string.IsNullOrEmpty(dto.Description)?string.Empty:dto.Description,
            p_Location=dto.Location.Trim().ToLower(),
            p_Website=dto.Website.Trim().ToLower()
        };
        int rowsaffected=await connection.ExecuteAsync("UpdateCompany",parameters,commandType:CommandType.StoredProcedure);
        if (rowsaffected == 0)
        {
            logger.LogCritical("Company Updation failed:Database responded with 0 rows affected");
            throw new Exception("Internal server error");
        }
        logger.LogInformation($"company {id} updated successfully");
        return true;
    }
}