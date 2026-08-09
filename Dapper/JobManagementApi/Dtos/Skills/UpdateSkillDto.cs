using System.ComponentModel.DataAnnotations;

namespace JobManagementApi.Dtos.Skills;

public record UpdateSkillDto(
    [Required]
    [MaxLength(100)]
    string Name
);