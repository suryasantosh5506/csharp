using Dapper;
using EmployeeManagementApi.Data;
using EmployeeManagementApi.Dtos.Company;
using EmployeeManagementApi.Entities;
using EmployeeManagementApi.Exceptions;
using EmployeeManagementApi.Extensions;
using EmployeeManagementApi.Interfaces;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.VisualBasic;

namespace EmployeeManagementApi.Services;

public class CompanyService(EmployeeContext context) : ICompanyService
{
    public async Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto createCompanyDto)
    {
        
        string existingquery="Select id from Company where email=@email";
        using var connection=context.GetConnection();
        int? id=null;
        id=await connection.QuerySingleOrDefaultAsync<int?>(existingquery,new{email=createCompanyDto.Email});
        if(id is not null) throw new ConflictException("Company already exists");
        string createQuery="Insert into Company (name,email,phone) Values(@name,@email,@phone)";
        int rowsaffected=await connection.ExecuteAsync(createQuery,new {name=createCompanyDto.Name,email=createCompanyDto.Email,phone=createCompanyDto.Phone});
        if(rowsaffected==0) throw new Exception("Internal Server Issue");
        Company company=await connection.QueryFirstAsync<Company>("Select * from Company where email=@email",new{email=createCompanyDto.Email});
        return company.ToDto();
    }

    public async Task<bool> DeleteCompanyAsync(int id)
    {
        using var connection=context.GetConnection();
        string existingquery="Select id from Company where Id=@id";
        int? existingid=null;
        existingid=await connection.QuerySingleOrDefaultAsync<int?>(existingquery,new{id=id});
        if(existingid is null) throw new NotFoundException("Company Not Found");
        string deletequery="Delete from Company where Id=@id";
        int rowsaffected=await connection.ExecuteAsync(deletequery,new{id=id});
        if(rowsaffected==0) throw new Exception("Internal Server Issue");
        return true;
    }

    public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync()
    {
        using var connection=context.GetConnection();
        var selecetQuery="Select * from Company";
        var companies=await connection.QueryAsync<Company>(selecetQuery);
        return companies.Select(x=>x.ToDto());
    }

    public async Task<CompanyDto?> GetCompanyByIdAsync(int id)
    {
        using var connection=context.GetConnection();
        var selectQuery="Select * from Company where id=@id";
        var company=await connection.QueryFirstOrDefaultAsync<Company>(selectQuery,new{id=id});
        if(company is null) throw new NotFoundException("Company Not Found");
        return company.ToDto();
    }

    public async Task<CompanyDetailsDto> GetCompanyDetailsAsync(int id)
    {
        using var connection=context.GetConnection();
        var query=@"select c.*,d.*
                from Company c left join Department d
                on c.Id=d.companyId
                where c.Id=@id";
        Dictionary<int,Company>companies=[];
        await connection.QueryAsync<Company,Department,Company>(query,
        (company, department) =>
        {
            if(!companies.TryGetValue(company.Id,out var existingCompany))
            {
                existingCompany=company;
                existingCompany.Departments=[];
                existingCompany.Employees=[];
                companies.Add(existingCompany.Id,existingCompany);
            }
            if (department is not null && !existingCompany.Departments.Any(x => x.Id == department.Id))
            {
                existingCompany.Departments.Add(department);
            }
            return existingCompany;
        },
        new {id=id},
        splitOn:"Id"
        );

        if(companies.Count==0) throw new NotFoundException("Company not found");
        Company company=companies.Values.First();
        return new CompanyDetailsDto(company.Id,
                company.Name,
                company.Email,
                company.Phone,
                company.Departments.Select(x=>x.ToDto()).ToList());
    } 
    public async Task<bool> UpdateCompanyAsync(int id, UpdateCompanyDto updateCompanyDto)
    {
        using var connection=context.GetConnection();
        string existquery="Select id from company where email=@email AND Id!=@id";
        var company=await connection.QueryFirstOrDefaultAsync<Company>(existquery,new {email=updateCompanyDto.Email,id=id});
        if(company is not null) throw new ConflictException("Company already exists");
        var updatequery="Update company set Name=@name,Email=@email,Phone=@phone where id=@id";
        int rowsaffected=await connection.ExecuteAsync(updatequery,new {name=updateCompanyDto.Name,email=updateCompanyDto.Email,phone=updateCompanyDto.Phone,id=id});
        if(rowsaffected==0) throw new Exception("Internal Server Issue");
        return true;
    }
}