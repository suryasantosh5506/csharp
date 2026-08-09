using System.ComponentModel.DataAnnotations;
using JobManagementApi.Enums;

namespace JobManagementApi.Dtos.JobApplication;

public record UpdateJobApplicationDto(
    [Required]
    ApplicationStatus Status
);