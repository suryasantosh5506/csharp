using System.ComponentModel.DataAnnotations;

namespace JobManagementApi.Dtos.Auth;

public record RegisterDto(
    [Required]
    [MaxLength(100)]
    string Name,
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    string Email,
    [Required]
    string Password
);