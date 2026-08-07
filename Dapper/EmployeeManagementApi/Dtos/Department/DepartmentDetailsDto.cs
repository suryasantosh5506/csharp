using EmployeeManagementApi.Dtos.Employee;

namespace EmployeeManagementApi.Dtos.Department;

public record DepartmentDetailsDto(
    int Id,
    string Name,
    int CompanyId,
    List<EmployeeDto> Employees
);