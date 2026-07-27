using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.Dtos.Author;

public record UpdateAuthorDto(
    [Required]
    [StringLength(50)]
    string Name,
    [Required]
    [EmailAddress]
    string Email,
    [Required]
    [StringLength(50)]
    string Country
);