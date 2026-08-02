using System.ComponentModel.DataAnnotations;

namespace LearnHubApi.Dtos.Lessons;

public record UpdateLessonDto(
    [Required]
    [MaxLength(200)]
    string Title,

    IFormFile? File,

    [MaxLength(1000)]
    string Description,

    [Range(1, int.MaxValue)]
    int Order,

    [Range(1, int.MaxValue)]
    int ModuleId
);