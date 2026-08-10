using System.ComponentModel.DataAnnotations;
using JobManagementApi.Enums;

namespace JobManagementApi.Dtos.RecruiterApplication;

public record UpdateRecruiterApplicationDto(
    [Required]
    RecruiterApplicationStatus Status
);
