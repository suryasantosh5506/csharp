using System.ComponentModel.DataAnnotations;

namespace JobManagementApi.Dtos.RecruiterApplication;

public record CreateRecruiterApplicationDto(
    [Required]
    [MaxLength(300)]
    string Reason
);
