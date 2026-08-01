using System.ComponentModel.DataAnnotations;

namespace LearnHubApi.Dtos.Category;

public record CreateCategoryDto(
    [Required]
    [MaxLength(100)]
    string Name,

    [MaxLength(500)]
    string Description
);