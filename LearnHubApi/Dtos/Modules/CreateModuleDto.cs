using System.ComponentModel.DataAnnotations;

namespace LearnHubApi.Dtos.Modules;

public record CreateModuleDto(
    [Required]
    [MaxLength(200)]
    string Title,

    [MaxLength(1000)]
    string Description,

    [Range(1, 1000)]
    int Order,

    [Range(1, int.MaxValue)]
    int CourseId
);