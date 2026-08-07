using EmployeeManagementApi.Dtos.Address;
using EmployeeManagementApi.RequestHelpers.Pagination;

namespace EmployeeManagementApi.Interfaces;

public interface IAddressService
{
    Task<AddressDto> CreateAddressAsync(CreateAddressDto dto);
    Task<AddressDto> GetAddressByIdAsync(int id);
    Task<PagedList<AddressDto>> GetAllAddressAsync(PaginationParams paginationParams);
    Task<AddressDto> GetAddressByEmployeeIdAsync(int empId);
    Task<bool> UpdateAddressAsync(int id,UpdateAddressDto dto);
    Task<bool> DeleteAddressAsync(int addressId);
}