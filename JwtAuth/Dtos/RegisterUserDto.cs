using System.ComponentModel.DataAnnotations;

namespace JwtAuth.Dtos;

public record RegisterUserDto(
    [Required]
    [StringLength(30)]
    string UserName,

    [Required]
    [MinLength(6)]
    string Password
);