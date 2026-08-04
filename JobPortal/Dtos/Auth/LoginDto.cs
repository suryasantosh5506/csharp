using System.ComponentModel.DataAnnotations;

namespace JobPortal.Dtos.Auth;

public record LoginDto(
    [Required]
    [EmailAddress]
    string Email,
    [Required]
    string Password
);