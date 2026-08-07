using EmployeeManagementApi.Dtos.Employee;

namespace EmployeeManagementApi.Dtos.Department;

public record DepartmentCompleteDto(
    int Id,
    string Name,
    List<EmployeeDetailsDto> Employees
);