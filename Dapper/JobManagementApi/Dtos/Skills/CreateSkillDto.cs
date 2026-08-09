using System.ComponentModel.DataAnnotations;

namespace JobManagementApi.Dtos.Skills;

public record CreateSkillDto(
    [Required]
    [MaxLength(100)]
    string Name
);