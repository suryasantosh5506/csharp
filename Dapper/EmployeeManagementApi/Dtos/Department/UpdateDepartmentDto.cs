using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementApi.Dtos.Department;

public record UpdateDepartmentDto(
    [Required]
    [MaxLength(100)]
    string Name,
    [Required]
    int CompanyId
);