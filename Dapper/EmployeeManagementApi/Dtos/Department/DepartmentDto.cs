using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementApi.Dtos.Department;

public record DepartmentDto(
    int Id,
    string Name,
    int CompanyId
);