using System.ComponentModel.DataAnnotations;

namespace LearnHubApi.Dtos.Authorization;

public record LoginDto(
    [EmailAddress]
    [Required]
    string Email,
    [Required]
    [MinLength(5)]
    [MaxLength(25)]
    string Password
);