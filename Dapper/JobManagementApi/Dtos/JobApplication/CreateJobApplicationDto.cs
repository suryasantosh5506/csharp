using System.ComponentModel.DataAnnotations;

namespace JobManagementApi.Dtos.JobApplication;

public record CreateJobApplicationDto(
    [Required]
    string ResumeUrl
);