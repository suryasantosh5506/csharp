using EmployeeManagementApi.Dtos.Employee;

namespace EmployeeManagementApi.Interfaces;
public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(int departmentId);

    Task<EmployeeDto?> GetEmployeeByIdAsync(int id);

    Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto);

    Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto);

    Task<bool> DeleteEmployeeAsync(int id);

    Task<EmployeeDetailsDto> GetEmployeeDetailsAsync(int id);
}