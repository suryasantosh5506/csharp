using EmployeeManagementApi.Dtos.Department;
namespace EmployeeManagementApi.Dtos.Company;
public record CompanyDetailsDto
(
    int Id,
    string Name,
    string Email,
    string Phone,
    List<DepartmentDto> Departments
);