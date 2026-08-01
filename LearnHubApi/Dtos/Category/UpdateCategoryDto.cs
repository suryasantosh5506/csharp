using System.ComponentModel.DataAnnotations;

namespace LearnHubApi.Dtos.Category;

public record UpdateCategoryDto(
    [Required]
    [MaxLength(100)]
    string Name,

    [MaxLength(500)]
    string Description
);