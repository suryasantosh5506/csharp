using System.ComponentModel.DataAnnotations;
namespace LibraryManagementAPI.Dtos.Author;
public record AuthorDetailsDto(
    int Id,
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