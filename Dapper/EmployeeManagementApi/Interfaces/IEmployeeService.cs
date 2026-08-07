using EmployeeManagementApi.Dtos.Employee;
using EmployeeManagementApi.Dtos.Transactions;
using EmployeeManagementApi.RequestHelpers.Pagination;
using EmployeeManagementApi.RequestHelpers.Search;

namespace EmployeeManagementApi.Interfaces;
public interface IEmployeeService
{
    Task<PagedList<EmployeeDto>> GetAllEmployeesAsync(int departmentId,EmployeeParams employeeParams);

    Task<EmployeeDto?> GetEmployeeByIdAsync(int id);

    Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto);

    Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto);

    Task<bool> DeleteEmployeeAsync(int id);

    Task<EmployeeDetailsDto> GetEmployeeDetailsAsync(int id);

    Task<EmployeeDetailsDto> CreateEmployeeWithAddressAsync(CreateEmployeeWithAddressDto dto);
}