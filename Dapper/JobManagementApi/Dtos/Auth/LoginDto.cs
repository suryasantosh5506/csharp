using System.ComponentModel.DataAnnotations;

namespace JobManagementApi.Dtos.Auth;

public record LoginDto(
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    string Email,
    [Required]
    string Password
);