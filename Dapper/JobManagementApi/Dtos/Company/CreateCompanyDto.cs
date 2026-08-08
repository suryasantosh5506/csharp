using System.ComponentModel.DataAnnotations;

namespace JobManagementApi.Dtos.Company;

public record CreateCompanyDto(
    [Required]
    [MaxLength(100)]
    string Name,
    string Description,
    [Required]
    [MaxLength(100)]
    string Location,
    [Required]
    [MaxLength(100)]
    string Website
);