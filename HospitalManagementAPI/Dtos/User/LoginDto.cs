using System.ComponentModel.DataAnnotations;
namespace HospitalManagementAPI.Dtos.User;
public record LoginDto(
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    string Email,

    [Required]
    string Password
);