using EmployeeManagementApi.Dtos.Address;

namespace EmployeeManagementApi.Interfaces;

public interface IAddressService
{
    Task<AddressDto> CreateAddressAsync(CreateAddressDto dto);
    Task<AddressDto> GetAddressByIdAsync(int id);
    Task<IEnumerable<AddressDto>> GetAllAddressAsync();
    Task<AddressDto> GetAddressByEmployeeIdAsync(int empId);
    Task<bool> UpdateAddressAsync(int id,UpdateAddressDto dto);
    Task<bool> DeleteAddressAsync(int addressId);
}