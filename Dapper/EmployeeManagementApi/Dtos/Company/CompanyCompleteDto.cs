using EmployeeManagementApi.Dtos.Department;

namespace EmployeeManagementApi.Dtos.Company;

public record CompanyCompleteDto(
    int Id,
    string Name,
    string Email,
    string Phone,
    List<DepartmentCompleteDto> Departments
);