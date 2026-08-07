using System.ComponentModel.DataAnnotations;
using System.Text;
using Dapper;
using EmployeeManagementApi.Data;
using EmployeeManagementApi.Dtos.Address;
using EmployeeManagementApi.Dtos.Employee;
using EmployeeManagementApi.Dtos.Transactions;
using EmployeeManagementApi.Entities;
using EmployeeManagementApi.Exceptions;
using EmployeeManagementApi.Extensions;
using EmployeeManagementApi.Interfaces;
using EmployeeManagementApi.RequestHelpers.Pagination;
using EmployeeManagementApi.RequestHelpers.Search;

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
    
    public async Task<PagedList<EmployeeDto>> GetAllEmployeesAsync(int departmentId,EmployeeParams employeeParams)
    {
        using var connection=context.GetConnection();
        var departmentQuery="select id from department where id=@did";
        int? dId=await connection.QueryFirstOrDefaultAsync<int?>(departmentQuery,new {did=departmentId});
        if(dId is null) throw new NotFoundException("Department not found");

        StringBuilder query=new StringBuilder();
        StringBuilder countQuery=new StringBuilder();

        query.Append("select * from Employee where departmentId=@did ");
        countQuery.Append("select count(*) from Employee where departmentId=@did ");

        var queryparams=new
        {
            did=departmentId,
            limit=employeeParams.PageSize,
            skip=(employeeParams.PageNumber-1)*employeeParams.PageSize,
            searchTerm=$"%{employeeParams.SearchTerm}%",
        };

        var order=(employeeParams.IsDescending)?"desc":"asc";

        if(!string.IsNullOrEmpty(employeeParams.SearchTerm))
        {
            query.Append("and name like @searchTerm ");
            countQuery.Append("and name like @searchTerm ");
        }

        if (!string.IsNullOrEmpty(employeeParams.SortBy))
        {
            query.Append(employeeParams.SortBy?.Trim()?.ToLower() switch
            {
                "name"=>$" order by name {order} ",
                "email"=>$" order by email {order} ",
                "phone"=>$"order by Phone {order} ",
                _ =>$" order by id {order} "
            });
        }
        else
        {
            query.Append($" order by id {order} ");
        }

        query.Append("limit @limit offset @skip");
        var employees=await connection.QueryAsync<Employee>(query.ToString(),queryparams);
        int count=await connection.ExecuteScalarAsync<int>(countQuery.ToString(),queryparams);
        return PagedList<EmployeeDto>.ToPagedList(employees.Select(x=>x.ToDto()),count,employeeParams.PageNumber,employeeParams.PageSize);
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

    public async Task<EmployeeDetailsDto> GetEmployeeDetailsAsync(int id)
    {
        using var connection=context.GetConnection();
        var query=@"Select e.*,a.* from
                    Employee e left join Address a
                    on e.Id=a.EmployeeId
                    where e.id=@id";
        var employees=(await connection.QueryAsync<Employee,Address,Employee>(
            query,
            (emp, add) =>
            {
                if(add is not null)
                {
                    emp.Address=add;
                }
                return emp;
            },
            new {id=id},
            splitOn:"Id"
        )).ToList();
        

        if(employees.Count==0) throw new NotFoundException("Employee not found");
        var employee=employees[0];
        return new EmployeeDetailsDto(
            employee.Id,
            employee.Name,
            employee.Email,
            employee.Phone,
            employee.CompanyId,
            employee.DepartmentId,
            employee.Address?.ToDto()
        );
    }

    public async Task<EmployeeDetailsDto> CreateEmployeeWithAddressAsync(CreateEmployeeWithAddressDto dto)
    {
        using var connection=context.GetConnection();
        connection.Open();
        using var transaction=connection.BeginTransaction();
        try
        {
            string companyQuery="select id from company where id=@id";
            int? cid=await connection.QueryFirstOrDefaultAsync<int?>(companyQuery,new {id=dto.Employee.CompanyId},transaction);
            if(cid is null) throw new NotFoundException("company not found");
            string deptQuery="Select id from department where id=@id and companyId=@cid";
            int? deptid=await connection.QueryFirstOrDefaultAsync<int?>(deptQuery,new {id=dto.Employee.DepartmentId,cid=dto.Employee.CompanyId},transaction:transaction);
            if(deptid is null) throw new NotFoundException("department not found");
            string empQuery="Select id from employee where email=@email";
            int? empId=await connection.QueryFirstOrDefaultAsync<int?>(empQuery,new {email=dto.Employee.Email},transaction:transaction);
            if(empId is not null) throw new ConflictException("Employee already exists");
            string insertQuery="insert into employee (name,email,phone,companyId,DepartmentId) values(@name,@email,@phone,@cid,@did)";
            var queryparams=new{
                name=dto.Employee.Name,
                email=dto.Employee.Email,
                phone=dto.Employee.Phone,
                cid=dto.Employee.CompanyId,
                did=dto.Employee.DepartmentId
            };
            int rowsaffected=await connection.ExecuteAsync(insertQuery,queryparams,transaction:transaction);
            if(rowsaffected==0) throw new Exception("Internal Server Error");
            empId=await connection.ExecuteScalarAsync<int>("select LAST_INSERT_ID()",transaction:transaction);

            string addressquery="insert into address (EmployeeId,HouseNo,Street,City,State,Country,PostalCode) values (@EmployeeId,@HouseNo,@Street,@City,@State,@Country,@PostalCode)";
            var insertparams = new
            {
                EmployeeId=empId,
                HouseNo=dto.Address.HouseNo,
                Street=dto.Address.Street,
                City=dto.Address.City,
                State=dto.Address.State,
                Country=dto.Address.Country,
                PostalCode=dto.Address.PostalCode
            };
            rowsaffected=await connection.ExecuteAsync(addressquery,insertparams,transaction:transaction);
            if(rowsaffected==0) throw new Exception("Internal Server Error");
            transaction.Commit();
            var employee=await connection.QuerySingleAsync<Employee>("Select * from employee where id=@id",new{id=empId});
            var address=await connection.QuerySingleAsync<Address>("Select * from address where employeeId=@id",new {id=empId});
            return new EmployeeDetailsDto(
                employee.Id,
                employee.Name,
                employee.Email,
                employee.Phone,
                employee.CompanyId,
                employee.DepartmentId,
                address?.ToDto()
            );
        }catch
        {
            transaction.Rollback();
            throw;
        }
    }
}