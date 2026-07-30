using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.Dtos.User;

public record RegisterDto(
    [Required]
    [MaxLength(50)]
    string FirstName,

    [Required]
    [MaxLength(50)]
    string LastName,

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    string Email,

    [Required]
    [MinLength(8)]
    [MaxLength(100)]
    string Password
);