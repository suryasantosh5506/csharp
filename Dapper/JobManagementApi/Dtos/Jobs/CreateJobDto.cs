using System.ComponentModel.DataAnnotations;
using JobManagementApi.Enums;

namespace JobManagementApi.Dtos.Jobs;

public record CreateJobDto(
    [Required]
    int CompanyId,
    [Required]
    [MaxLength(500)]
    string Title,
    string Description,
    [Required]
    string Location,
    [Required]
    decimal SalaryMin,
    [Required]
    decimal SalaryMax,
    [Required]
    JobTypes JobType,
    [Required]
    int Experience
);