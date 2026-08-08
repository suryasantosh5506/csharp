using System.Data;
using Dapper;
using EmployeeManagementApi.Data;
using EmployeeManagementApi.Dtos.Company;
using EmployeeManagementApi.Dtos.Department;
using EmployeeManagementApi.Dtos.Employee;
using EmployeeManagementApi.Entities;
using EmployeeManagementApi.Exceptions;
using EmployeeManagementApi.Extensions;
using EmployeeManagementApi.Interfaces;
using EmployeeManagementApi.RequestHelpers.Pagination;
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
        int rowsaffected=await connection.ExecuteAsync("CreateCompany",new {p_Name=createCompanyDto.Name,p_Email=createCompanyDto.Email,p_Phone=createCompanyDto.Phone},commandType: CommandType.StoredProcedure);
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
        
        int rowsaffected=await connection.ExecuteAsync("DeleteCompany",new{p_Id=id},commandType: CommandType.StoredProcedure);
        if(rowsaffected==0) throw new Exception("Internal Server Issue");
        return true;
    }

    public async Task<PagedList<CompanyDto>> GetAllCompaniesAsync(PaginationParams paginationParams)
    {
        using var connection=context.GetConnection();
        var selecetQuery="Select * from Company limit @limit offset @skip";
        var queryParams=new{limit=paginationParams.PageSize,skip=(paginationParams.PageNumber-1)*paginationParams.PageSize};
        var companies=await connection.QueryAsync<Company>(selecetQuery,queryParams);
        int count=await connection.ExecuteScalarAsync<int>("Select count(*) from Company");
        return PagedList<CompanyDto>.ToPagedList(companies.Select(x=>x.ToDto()),count,paginationParams.PageNumber,paginationParams.PageSize);
    }

    public async Task<CompanyDto?> GetCompanyByIdAsync(int id)
    {
        using var connection=context.GetConnection();
        var company=await connection.QueryFirstOrDefaultAsync<Company>("GetCompanyById",new{CompanyId=id},commandType:CommandType.StoredProcedure);
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
        
        int rowsaffected=await connection.ExecuteAsync("UpdateCompany",new {p_Name=updateCompanyDto.Name,p_Email=updateCompanyDto.Email,p_Phone=updateCompanyDto.Phone,p_Id=id},commandType: CommandType.StoredProcedure);
        if(rowsaffected==0) throw new Exception("Internal Server Issue");
        return true;
    }

    public async Task<CompanyCompleteDto> GetCompanyCompleteAsync(int id)
    {
        using var connection=context.GetConnection();
        var query=@"select c.*,d.*,e.*,a.* from
                    Company c left join Department d
                    on c.Id=d.CompanyId
                    left join Employee e
                    on d.Id=e.DepartmentId
                    left join address a
                    on e.Id=a.EmployeeId
                    where c.Id=@id";
        
        Dictionary<int,Company>companiesDict=[]; 
        Dictionary<int,Department>departmentsDict=[]; 
        await connection.QueryAsync<Company,Department,Employee,Address,Company>( 
            query, 
            (company, department, employee, address) => { 
                if(!companiesDict.TryGetValue(company.Id,out var existingCompany)) { 
                    existingCompany=company; existingCompany.Departments=[]; 
                    companiesDict.Add(company.Id,existingCompany); 
                }

                if (department.Id != 0)
                {
                    if(!departmentsDict.TryGetValue(department.Id,out var existingDepartment)) { 
                        existingDepartment=department; 
                        existingDepartment.Employees=[]; 
                        departmentsDict.Add(department.Id,existingDepartment); 
                    } 

                    if (!existingCompany.Departments.Any(x => x.Id == existingDepartment.Id)) { 
                        existingCompany.Departments.Add(existingDepartment); 
                    }
                    
                    if (employee.Id != 0 && !existingDepartment.Employees.Any(x => x.Id == employee.Id)) { 
                        existingDepartment.Employees.Add(employee);
                        if(address is not null) { 
                            employee.Address=address; 
                        }
                    }
                } 
                return existingCompany; 
                }, 
            new {id=id}, 
            splitOn:"Id,Id,Id" 
        );

        if(companiesDict.Count==0) throw new NotFoundException("comapny not found");
        var company=companiesDict.Values.First();
        var departments=departmentsDict.Values.ToList();

        var deptCompleteDto=departments.Select(x=>
                        new DepartmentCompleteDto(x.Id,x.Name,x.Employees
                        .Select(emp=>new EmployeeDetailsDto(emp.Id,emp.Name,emp.Email,emp.Phone,emp.CompanyId,emp.DepartmentId,emp.Address?.ToDto())).ToList()
                        )).ToList();

        return new CompanyCompleteDto(
            company.Id,
            company.Name,
            company.Email,
            company.Phone,
            deptCompleteDto
        );
    }

    public async Task<CompanySummaryDto> GetCompanySummaryAsync(int id)
    {
        var connection=context.GetConnection();
        string query=@"Select * from company where id=@id;

                        select * from department where companyId=@id;

                        select count(*) from Employee where companyId=@id;
                    ";

        using var multi=await connection.QueryMultipleAsync(query,new{id=id});
        Company? company=await multi.ReadFirstOrDefaultAsync<Company?>();
        if(company is null) throw new NotFoundException("Company not found");
        var departments=await multi.ReadAsync<Department>();
        int cnt=await multi.ReadSingleAsync<int>();

        return new CompanySummaryDto(
                new CompanyDetailsDto(
                    company.Id,
                    company.Name,
                    company.Email,
                    company.Phone,
                    departments.Select(x=>x.ToDto()).ToList()),
                    cnt);
    }
}