using System.Data;
using Dapper;
using EmployeeManagementApi.Data;
using EmployeeManagementApi.Dtos.Department;
using EmployeeManagementApi.Entities;
using EmployeeManagementApi.Exceptions;
using EmployeeManagementApi.Extensions;
using EmployeeManagementApi.Interfaces;
using EmployeeManagementApi.RequestHelpers.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementApi.Services;

public class DepartmentService(EmployeeContext context) : IDepartmentService
{
    public async Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto dto)
    {
        string existquery="Select id from department where companyid=@companyid and name=@name";
        using var connection=context.GetConnection();
        int ?id=null;
        id=await connection.QueryFirstOrDefaultAsync<int?>(existquery,new{companyid=dto.CompanyId,name=dto.Name});
        if(id is not null) throw new ConflictException("Department already exists");
        var companyquery="Select * from company where id=@id";
        var company=await connection.QueryFirstOrDefaultAsync<Company>(companyquery,new{id=dto.CompanyId});
        if(company is null) throw new NotFoundException("Company not found");
        
        int rowsaffected=await connection.ExecuteAsync("CreateDepartment",new {p_Name=dto.Name,p_CompanyId=dto.CompanyId},commandType: CommandType.StoredProcedure);
        if(rowsaffected==0) throw new Exception("Internal Server Error");
        var department=await connection.QueryFirstAsync<Department>("Select * from department where companyid=@id and name=@name",new {id=dto.CompanyId,name=dto.Name});
        return department.ToDto();
    }

    public async Task<bool> DeleteDepartmentAsync(int id)
    {
        string existquery="Select id from department where id=@id";
        using var connection=context.GetConnection();
        int ?existingId=null;
        existingId=await connection.QueryFirstOrDefaultAsync<int?>(existquery,new{id=id});
        if(existingId is null) throw new NotFoundException("Department Not Found");
        
        int rowsaffected=await connection.ExecuteAsync("DeleteDepartment",new {p_Id=id},commandType: CommandType.StoredProcedure);
        if(rowsaffected==0) throw new Exception("Internal Server Error");
        return true;
    }

    public async Task<PagedList<DepartmentDto>> GetAllDepartmentsAsync(int companyId,PaginationParams paginationParams)
    {
        using var connection=context.GetConnection();
        var companyquery="Select * from company where id=@id";
        var queryParams=new{id=companyId,limit=paginationParams.PageSize,skip=(paginationParams.PageNumber-1)*paginationParams.PageSize};
        var company=await connection.QueryFirstOrDefaultAsync<Company>(companyquery,new {id=companyId});
        if(company is null) throw new NotFoundException("Company not found");
        int count=await connection.ExecuteScalarAsync<int>("select count(*) from department where companyId=@id",new{id=companyId});
        var departments=await connection.QueryAsync<Department>("Select * from department where companyId=@id limit @limit offset @skip",queryParams);
        return PagedList<DepartmentDto>.ToPagedList(departments.Select(x=>x.ToDto()),count,paginationParams.PageNumber,paginationParams.PageSize);
    }

    public async Task<DepartmentDto?> GetDepartmentByIdAsync(int id)
    {
        using var connection=context.GetConnection();
        var department=await connection.QueryFirstOrDefaultAsync<Department>("GetDepartmentById",new {DepartmentId=id},commandType:CommandType.StoredProcedure);
        if(department is null) throw new NotFoundException("department not found");
        return department.ToDto();
    }


    public async Task<bool> UpdateDepartmentAsync(int id, UpdateDepartmentDto dto)
    {
        using var connection=context.GetConnection();
        var companyquery="Select * from company where id=@id";
        var company=await connection.QueryFirstOrDefaultAsync<Company>(companyquery,new{id=dto.CompanyId});
        if(company is null) throw new NotFoundException("Company not found");
        const string departmentQuery = "SELECT Id FROM Department WHERE Id = @Id";
        int? departmentId = await connection.QueryFirstOrDefaultAsync<int?>(departmentQuery,new { Id = id });
        if (departmentId is null) throw new NotFoundException("Department not found");
        string existquery="Select id from department where companyid=@companyid and name=@name and id<>@id";
        int ?existId=null;
        existId=await connection.QueryFirstOrDefaultAsync<int?>(existquery,new{companyid=dto.CompanyId,name=dto.Name,id=id});
        if(existId is not null) throw new ConflictException("Department already exists");
        
        int rowsaffected=await connection.ExecuteAsync("UpdateDepartment",new{p_Name=dto.Name,p_CompanyId=dto.CompanyId,p_Id=id},commandType: CommandType.StoredProcedure);
        if(rowsaffected==0) throw new Exception("Internal Server Error");
        return true;
    }

    public async Task<DepartmentDetailsDto> GetDepartmentDetailsAsync(int id)
    {
        using var connection=context.GetConnection();
        var query=@"select d.*,e.* from
                    department d left join Employee e
                    on d.Id=e.departmentId
                    where d.Id=@id";
        
        Dictionary<int,Department>departments=[];
        await connection.QueryAsync<Department,Employee,Department>(
            query,
            (dept, emp) =>
            {
                if(!departments.TryGetValue(dept.Id,out var existingDepartment))
                {   
                    existingDepartment=dept;
                    existingDepartment.Employees=[];
                    departments.Add(dept.Id,existingDepartment);
                }

                if(emp is not null && !existingDepartment.Employees.Any(x => x.Email == emp.Email))
                {
                    existingDepartment.Employees.Add(emp);
                }

                return existingDepartment;
            },
            new {id=id},
            splitOn:"Id"
        );
        if(departments.Count==0) throw new NotFoundException("Department not found");
        var department=departments.Values.First();
        return new DepartmentDetailsDto(department.Id,department.Name,department.CompanyId,department.Employees.Select(x=>x.ToDto()).ToList());
    }
}