using System.ComponentModel.DataAnnotations;

namespace LearnHubApi.Dtos.Reviews;

public record UpdateReviewDto(
    [Range(1, 5)]
    double Rating,

    [Required]
    [MaxLength(1000)]
    string Comment
);