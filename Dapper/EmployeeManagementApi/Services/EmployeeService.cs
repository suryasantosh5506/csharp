using System.ComponentModel.DataAnnotations;
using Dapper;
using EmployeeManagementApi.Data;
using EmployeeManagementApi.Dtos.Employee;
using EmployeeManagementApi.Entities;
using EmployeeManagementApi.Exceptions;
using EmployeeManagementApi.Extensions;
using EmployeeManagementApi.Interfaces;

namespace EmployeeManagementApi.Services;

public class EmployeeService(EmployeeContext context) : IEmployeeService
{
    public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto)
    {
        using var connection=context.GetConnection();
        var companyquery="select id from company where id=@id";
        int? companyId=await connection.QueryFirstOrDefaultAsync<int?>(companyquery,new{id=dto.CompanyId});
        if(companyId is null) throw new NotFoundException("Company not found");
        var departmentQuery="select id from department where companyId=@cid and id=@id";
        int? departmentId=await connection.QueryFirstOrDefaultAsync<int?>(departmentQuery,new {cid=dto.CompanyId,id=dto.DepartmentId});
        if(departmentId is null) throw new NotFoundException("Department not found");
        var existquery="Select id from Employee where email=@email";
        int? id=await connection.QueryFirstOrDefaultAsync<int?>(existquery,new {email=dto.Email});
        if(id is not null) throw new ConflictException("Employee already exists");
        var insertquery="insert into employee (name,email,phone,companyId,departmentId) values(@name,@email,@phone,@cid,@did)";
        int rowsaffected=await connection.ExecuteAsync(insertquery,new {name=dto.Name,email=dto.Email,phone=dto.Phone,cid=dto.CompanyId,did=dto.DepartmentId});
        if(rowsaffected==0) throw new Exception("Internal Server Error");
        Employee employee=await connection.QueryFirstAsync<Employee>("select * from employee where email=@email",new{email=dto.Email});
        return employee.ToDto();
    }

    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        using var connection=context.GetConnection();
        var employeeQuery="select id from employee where id=@id";
        int? employeeId=await connection.QueryFirstOrDefaultAsync<int?>(employeeQuery,new{id=id});
        if(employeeId is null) throw new NotFoundException("Employee not found");
        var deletequery="delete from employee where id=@id";
        int rowsaffected=await connection.ExecuteAsync(deletequery,new{id=id});
        if(rowsaffected==0) throw new Exception("Internal Server Error");
        return true;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(int departmentId)
    {
        using var connection=context.GetConnection();
        var departmentQuery="select id from department where id=@did";
        int? dId=await connection.QueryFirstOrDefaultAsync<int?>(departmentQuery,new {did=departmentId});
        if(dId is null) throw new NotFoundException("Department not found");
        var employees=await connection.QueryAsync<Employee>("Select * from Employee where departmentId=@did",new{did=departmentId});
        return employees.Select(x=>x.ToDto());
    }

    public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
    {
        using var connection=context.GetConnection();
        var employeeQuery="select * from employee where id=@id";
        Employee? employee=await connection.QueryFirstOrDefaultAsync<Employee?>(employeeQuery,new {id=id});
        if(employee is null) throw new NotFoundException("employee not found");
        return employee.ToDto();
    }

    public async Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto)
    {
        using var connection=context.GetConnection();
        var employeeQuery="select id from employee where id=@id";
        int? employeeId=await connection.QueryFirstOrDefaultAsync<int?>(employeeQuery,new{id=id});
        if(employeeId is null) throw new NotFoundException("Employee not found");
        var companyquery="select id from company where id=@id";
        int? companyId=await connection.QueryFirstOrDefaultAsync<int?>(companyquery,new{id=dto.CompanyId});
        if(companyId is null) throw new NotFoundException("Company not found");
        var departmentQuery="select id from department where companyId=@cid and id=@id";
        int? departmentId=await connection.QueryFirstOrDefaultAsync<int?>(departmentQuery,new {cid=dto.CompanyId,id=dto.DepartmentId});
        if(departmentId is null) throw new NotFoundException("Department not found");
        var existquery="Select id from Employee where email=@email and id<>@id";
        int? eid=await connection.QueryFirstOrDefaultAsync<int?>(existquery,new {email=dto.Email,id=id});
        if(eid is not null) throw new ConflictException("Employee already exists");
        var updatequery="update employee set name=@name,email=@email,phone=@phone,companyId=@cid,departmentId=@did where id=@id";
        int rowsaffected=await connection.ExecuteAsync(updatequery,new {name=dto.Name,email=dto.Email,phone=dto.Phone,cid=dto.CompanyId,did=dto.DepartmentId,id=id});
        if(rowsaffected==0) throw new Exception("Internal Server Error");
        return true;
    }
}