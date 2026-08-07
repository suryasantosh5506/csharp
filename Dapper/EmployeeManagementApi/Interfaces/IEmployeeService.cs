using EmployeeManagementApi.Dtos.Employee;
using EmployeeManagementApi.Dtos.Transactions;
using EmployeeManagementApi.RequestHelpers.Pagination;

namespace EmployeeManagementApi.Interfaces;
public interface IEmployeeService
{
    Task<PagedList<EmployeeDto>> GetAllEmployeesAsync(int departmentId,PaginationParams paginationParams);

    Task<EmployeeDto?> GetEmployeeByIdAsync(int id);

    Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto);

    Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto);

    Task<bool> DeleteEmployeeAsync(int id);

    Task<EmployeeDetailsDto> GetEmployeeDetailsAsync(int id);

    Task<EmployeeDetailsDto> CreateEmployeeWithAddressAsync(CreateEmployeeWithAddressDto dto);
}