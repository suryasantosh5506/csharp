using System.ComponentModel.DataAnnotations;
using LearnHubApi.Enums;

namespace LearnHubApi.Dtos.Authorization;

public record LoginResponseDto(
    [Required]
    string Token,
    [Required]
    string FirstName,
    [Required]
    string LastName,
    [EmailAddress]
    [Required]
    string Email,
    [Required]
    UserRole Role,
    int Id
);