using EmployeeManagementApi.Dtos.Address;
using EmployeeManagementApi.Dtos.Company;
using EmployeeManagementApi.Entities;

namespace EmployeeManagementApi.Extensions;

public static class AddressExtension{
    public static AddressDto ToDto(this Address address)
    {
        return new(address.Id,address.EmployeeId,address.HouseNo,address.Street,address.City,address.State,address.Country,address.PostalCode);
    }
}