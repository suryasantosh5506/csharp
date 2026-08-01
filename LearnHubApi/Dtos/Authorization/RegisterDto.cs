using System.ComponentModel.DataAnnotations;

namespace LearnHubApi.Dtos.Authorization;

public record RegisterDto(
    [EmailAddress]
    [Required]
    string Email,
    [Required]
    [MinLength(5)]
    [MaxLength(25)]
    string Password,
    [Required]
    string FirstName,
    [Required]
    string LastName
);