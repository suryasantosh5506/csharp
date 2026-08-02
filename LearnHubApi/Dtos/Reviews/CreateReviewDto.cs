using System.ComponentModel.DataAnnotations;

namespace LearnHubApi.Dtos.Reviews;

public record CreateReviewDto(
    [Range(1, 5)]
    double Rating,

    [Required]
    [MaxLength(1000)]
    string Comment,

    [Range(1, int.MaxValue)]
    int CourseId
);