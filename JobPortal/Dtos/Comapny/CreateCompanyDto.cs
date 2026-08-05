using System.ComponentModel.DataAnnotations;

namespace JobPortal.Dtos.Company;

public record CreateCompanyDto
(
    [Required]
    string Name,
    [Required]
    string Description,
    [Required]
    string Website,
    [Required]
    string LogoUrl
);