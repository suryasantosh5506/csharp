using System.ComponentModel.DataAnnotations;

namespace JobPortal.Dtos.Auth;

public record RegisterDto(
    [Required]
    string FullName,
    [Required]
    [EmailAddress]
    string Email,
    [Required]
    string Password,
    [Url]
    string? ProfileImageUrl
);