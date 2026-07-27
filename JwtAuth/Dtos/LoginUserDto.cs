using System.ComponentModel.DataAnnotations;

namespace JwtAuth.Dtos;

public record LoginUserDto(
    [Required]
    string UserName,

    [Required]
    string Password
);