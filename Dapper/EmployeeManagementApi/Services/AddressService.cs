using EmployeeManagementApi.Entities;
using Dapper;
using EmployeeManagementApi.Data;
using EmployeeManagementApi.Dtos.Address;
using EmployeeManagementApi.Exceptions;
using EmployeeManagementApi.Interfaces;
using EmployeeManagementApi.Extensions;
using EmployeeManagementApi.RequestHelpers.Pagination;

namespace EmployeeManagementApi.Services;

public class AddressService(EmployeeContext context) : IAddressService
{
    public async Task<AddressDto> CreateAddressAsync(CreateAddressDto dto)
    {
        using var connection=context.GetConnection();
        var employeeQuery="select id from employee where id=@id";
        int? id=await connection.QueryFirstOrDefaultAsync<int?>(employeeQuery,new {id=dto.EmployeeId});
        if(id is null) throw new NotFoundException("Employee not found");
        var existAddressQuery="select id from address where employeeId=@id";
        int? addId=await connection.QueryFirstOrDefaultAsync<int?>(existAddressQuery,new {id=dto.EmployeeId});
        if(addId is not null) throw new ConflictException("Address already associated with the employee");
        var insertQuery="insert into address (EmployeeId,HouseNo,Street,City,State,Country,PostalCode) values(@EmployeeId,@HouseNo,@Street,@City,@State,@Country,@PostalCode)";
        var queryparams=new
        {
            EmployeeId=dto.EmployeeId,
            HouseNo=dto.HouseNo,
            Street=dto.Street,
            City=dto.City,
            State=dto.State,
            Country=dto.Country,
            PostalCode=dto.PostalCode
        };
        int rowsaffected=await connection.ExecuteAsync(insertQuery,queryparams);
        if(rowsaffected==0) throw new Exception("Internal Server error");
        var address=await connection.QueryFirstAsync<Address>("Select * from Address where employeeid=@id",new{id=dto.EmployeeId});
        return address.ToDto();
    }

    public async Task<bool> DeleteAddressAsync(int addressId)
    {
        using var connection=context.GetConnection();
        var addressquery="Select id from address where id=@id";
        var addressParams=new {id=addressId};
        int? addId=await connection.QueryFirstOrDefaultAsync<int?>(addressquery,addressParams);
        if(addId is null) throw new NotFoundException("Address not found");
        var deleteQuery="delete from address where id=@id";
        int rowsaffected=await connection.ExecuteAsync(deleteQuery,addressParams);
        if(rowsaffected==0) throw new Exception("Internal Server error");
        return true;
    }

    public async Task<AddressDto> GetAddressByEmployeeIdAsync(int empId)
    {
        using var connection=context.GetConnection();
        var employeeQuery="select id from employee where id=@id";
        int? id=await connection.QueryFirstOrDefaultAsync<int?>(employeeQuery,new {id=empId});
        if(id is null) throw new NotFoundException("Employee not found");
        var addressquery="Select * from address where EmployeeId=@id";
        var addressParams=new {id=empId};
        Address? address=await connection.QueryFirstOrDefaultAsync<Address?>(addressquery,addressParams);
        if(address is null) throw new NotFoundException($"Employee with {empId} has no associated address");
        return address.ToDto();
    }

    public async Task<AddressDto> GetAddressByIdAsync(int id)
    {
        using var connection=context.GetConnection();
        var addressquery="Select * from address where id=@id";
        var addressParams=new {id=id};
        Address? address=await connection.QueryFirstOrDefaultAsync<Address?>(addressquery,addressParams);
        if(address is null) throw new NotFoundException("Address not found");
        return address.ToDto();
    }

    public async Task<PagedList<AddressDto>> GetAllAddressAsync(PaginationParams paginationParams)
    {
        using var connection=context.GetConnection();
        var selectQuery="Select * from address limit @limit offset @skip";
        int count=await connection.ExecuteScalarAsync<int>("select count(*) from Address");
        var queryParams=new{limit=paginationParams.PageSize,skip=(paginationParams.PageNumber-1)*paginationParams.PageSize};
        var addresses=await connection.QueryAsync<Address>(selectQuery,queryParams);
        return PagedList<AddressDto>.ToPagedList(addresses.Select(x=>x.ToDto()),count,paginationParams.PageNumber,paginationParams.PageSize);
    }

    public async Task<bool> UpdateAddressAsync(int id,UpdateAddressDto dto)
    {
        using var connection=context.GetConnection();
        var existAddressQuery="select id from address where id=@id";
        int? addId=await connection.QueryFirstOrDefaultAsync<int?>(existAddressQuery,new {id=id});
        if(addId is null) throw new NotFoundException("Address Not Found");
        var updateQuery="update address set HouseNo=@HouseNo,Street=@Street,City=@City,State=@State,Country=@Country,PostalCode=@PostalCode where id=@id";

        var queryparams=new
        {
            HouseNo=dto.HouseNo,
            Street=dto.Street,
            City=dto.City,
            State=dto.State,
            Country=dto.Country,
            PostalCode=dto.PostalCode,
            id=id
        };
        int rowsaffected=await connection.ExecuteAsync(updateQuery,queryparams);
        if(rowsaffected==0) throw new Exception("Internal Server error");
        return true;
    }
}