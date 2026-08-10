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

namespace JobManagementApi.Services;

public class CompanyService(DapperContext context, ICurrentUserService currentUser) : ICompanyService
{
    public async Task<CompanyDto> CreateCompany(CreateCompanyDto dto)
    {
        if(!currentUser.IsAuthenticated) throw new UnauthorizedException("Unauthorized");
        if(currentUser.Role!=UserRole.Admin && currentUser.Role != UserRole.Recruiter)
        {
            throw new ForbiddenException("Only admin and recruiter can create a company");
        }
        using var connection=context.GetConnection();
        string query="Select * from company where name=@name";
        Company? existingCompany=await connection.QueryFirstOrDefaultAsync<Company?>(query,new{name=dto.Name.Trim().ToLower()});
        if(existingCompany is not null)
        {
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
        if(rowsaffected==0) throw new Exception("Internal server error");
        var company=await connection.QueryFirstAsync<Company>("select * from company where name=@name",new{name=dto.Name.Trim().ToLower()});
        return company.ToDto();
    }

    public async Task<bool> DeleteCompany(int id)
    {
        if(!currentUser.IsAuthenticated) throw new UnauthorizedException("Unauthorized");
        if(currentUser.Role!=UserRole.Admin && currentUser.Role != UserRole.Recruiter)
        {
            throw new ForbiddenException("Only admin and recruiter can delete a company");
        }
        using var connection=context.GetConnection();
        var company=await connection.QueryFirstOrDefaultAsync<Company?>("GetCompanyById",new{p_Id=id},commandType:CommandType.StoredProcedure);
        if(company is null) throw new NotFoundException("Company not found");
        if(company.UserId!=currentUser.UserId && currentUser.Role != UserRole.Admin)
        {
            throw new ForbiddenException("Only admin and recruiter can delete a company");
        }
        int rowsaffected=await connection.ExecuteAsync("DeleteCompanyById",new{p_Id=id},commandType:CommandType.StoredProcedure);
        if(rowsaffected==0) throw new Exception("Internal server error");
        return true;
    }

    public async Task<PagedList<CompanyDto>> GetCompanies(PaginationParams paginationParams)
    {
        using var connection=context.GetConnection();
        StringBuilder query=new StringBuilder();
        query.Append("Select * from company order by id asc ");
        query.Append("limit @limit offset @offset");
        var parameters=new{limit=paginationParams.PageSize,offset=(paginationParams.PageNumber-1)*paginationParams.PageSize};
        var companies=await connection.QueryAsync<Company>(query.ToString(),parameters);
        int totalCount=await connection.ExecuteScalarAsync<int>("select count(*) from company");
        return PagedList<CompanyDto>.ToPagedList(companies.Select(x=>x.ToDto()),paginationParams.PageNumber,totalCount,paginationParams.PageSize);
    }

    public async Task<CompanyDto> GetCompanyById(int id)
    {
        using var connection=context.GetConnection();
        var company=await connection.QueryFirstOrDefaultAsync<Company?>("GetCompanyById",new{p_Id=id},commandType:CommandType.StoredProcedure);
        if(company is null) throw new NotFoundException("Company not found");
        return company.ToDto();
    }
    public async Task<bool> UpdateCompany(int id, UpdateCompanyDto dto)
    {
        if(!currentUser.IsAuthenticated) throw new UnauthorizedException("Unauthorized");
        if(currentUser.Role!=UserRole.Admin && currentUser.Role != UserRole.Recruiter)
        {
            throw new ForbiddenException("Only admin and recruiter can update a company");
        }
        using var connection=context.GetConnection();
        
        Company? existingCompany=await connection.QueryFirstOrDefaultAsync<Company?>("GetCompanyById",new{p_Id=id},commandType:CommandType.StoredProcedure);

        if(existingCompany is null)
        {
            throw new NotFoundException("Company Not found");
        }

        if (existingCompany.UserId != currentUser.UserId && currentUser.Role != UserRole.Admin)
        {
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
        if(rowsaffected==0) throw new Exception("Internal server error");
        return true;
    }
}