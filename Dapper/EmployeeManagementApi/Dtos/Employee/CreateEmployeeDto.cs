using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementApi.Dtos.Employee;

public record CreateEmployeeDto(
    [Required]
    [MaxLength(100)]
    string Name,
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    string Email,
    [Required]
    [MaxLength(20)]
    [Phone]
    string Phone,
    [Required]
    int CompanyId,
    [Required]
    int DepartmentId
);