using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementApi.Dtos.Company;

public record CreateCompanyDto(
    [Required]
    [MaxLength(100)]
    string Name,
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    string Email,
    [Required]
    [Phone]
    [MaxLength(20)]
    string Phone
);