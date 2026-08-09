using System.ComponentModel.DataAnnotations;
using JobManagementApi.Enums;

namespace JobManagementApi.Dtos.Jobs;

public record CreateJobDto(
    [Required]
    int CompanyId,
    [Required]
    [MaxLength(300)]
    string Title,
    string Description,
    [Required]
    [MaxLength(100)]
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