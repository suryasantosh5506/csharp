using System.ComponentModel.DataAnnotations;

namespace LearnHubApi.Dtos.Lessons;

public record CreateLessonDto(
    [Required]
    [MaxLength(200)]
    string Title,
    [Required]
    IFormFile File,
    [MaxLength(1000)]
    string Description,

    [Range(1, int.MaxValue)]
    int Order,

    [Range(1, int.MaxValue)]
    int ModuleId
);