using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.Dtos.Category;

public record CreateCategoryDto(
    [Required(ErrorMessage = "Category name is required.")]
    [StringLength(50, MinimumLength = 3,
        ErrorMessage = "Category name must be between 3 and 50 characters.")]
    string Name
);