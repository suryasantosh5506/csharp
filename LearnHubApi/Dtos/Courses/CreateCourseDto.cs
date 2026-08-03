using System.ComponentModel.DataAnnotations;

namespace LearnHubApi.Dtos.Courses;

public record CreateCourseDto(
    [Required]
    [MaxLength(200)]
    string Title,

    [Required]
    [MaxLength(2000)]
    string Description,

    [Required]
    IFormFile Thumbnail,

    [Range(0, double.MaxValue)]
    decimal Price,

    [Required]
    [MaxLength(50)]
    string Language,

    [Range(0.1, double.MaxValue)]
    double Duration,

    [Range(1, int.MaxValue)]
    int CategoryId
);