using System.ComponentModel.DataAnnotations;

namespace JobManagementApi.Dtos.Company;

public record CreateCompanyDto(
    [Required]
    [MaxLength(100)]
    string Name,
    string Description,
    [Required]
    [MaxLength(500)]
    string Location,
    [Required]
    [MaxLength(500)]
    string Website
);