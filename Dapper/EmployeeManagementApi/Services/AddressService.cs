using EmployeeManagementApi.Entities;
using Dapper;
using EmployeeManagementApi.Data;
using EmployeeManagementApi.Dtos.Address;
using EmployeeManagementApi.Exceptions;
using EmployeeManagementApi.Interfaces;
using EmployeeManagementApi.Extensions;
using EmployeeManagementApi.RequestHelpers.Pagination;
using System.Data;

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

        var queryparams=new
        {
            p_EmployeeId=dto.EmployeeId,
            p_HouseNo=dto.HouseNo,
            p_Street=dto.Street,
            p_City=dto.City,
            p_State=dto.State,
            p_Country=dto.Country,
            p_PostalCode=dto.PostalCode
        };
        int rowsaffected=await connection.ExecuteAsync("CreateAddress",queryparams,commandType:CommandType.StoredProcedure);
        if(rowsaffected==0) throw new Exception("Internal Server error");
        var address=await connection.QueryFirstAsync<Address>("Select * from Address where employeeid=@id",new{id=dto.EmployeeId});
        return address.ToDto();
    }

    public async Task<bool> DeleteAddressAsync(int addressId)
    {
        using var connection=context.GetConnection();
        var addressquery="Select id from address where id=@id";
        var addressParams=new {p_Id=addressId};
        int? addId=await connection.QueryFirstOrDefaultAsync<int?>(addressquery,addressParams);
        if(addId is null) throw new NotFoundException("Address not found");
        int rowsaffected=await connection.ExecuteAsync("DeleteAddress",addressParams,commandType: CommandType.StoredProcedure);
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
        var addressParams=new {AddressId=id};
        Address? address=await connection.QueryFirstOrDefaultAsync<Address?>("GetAddressById",addressParams,commandType:CommandType.StoredProcedure);
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
       

        var queryparams=new
        {
            p_HouseNo=dto.HouseNo,
            p_Street=dto.Street,
            p_City=dto.City,
            p_State=dto.State,
            p_Country=dto.Country,
            p_PostalCode=dto.PostalCode,
            p_Id=id
        };
        int rowsaffected=await connection.ExecuteAsync("UpdateAddress",queryparams,commandType: CommandType.StoredProcedure);
        if(rowsaffected==0) throw new Exception("Internal Server error");
        return true;
    }
}