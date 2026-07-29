using System.ComponentModel.DataAnnotations;

namespace JwtAuth.Dtos;

public record UserDto(
    [Required]
    [StringLength(30)]
    string UserName,
    [Required]
    [StringLength(20)]
    string Password
);