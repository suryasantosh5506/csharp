using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementApi.Dtos.Department;

public record CreateDepartmentDto(
    [Required]
    [MaxLength(100)]
    string Name,
    [Required]
    int CompanyId
);