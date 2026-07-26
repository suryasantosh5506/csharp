using System.ComponentModel.DataAnnotations;

namespace VideoGameCharacterApi.Dtos;

public record CreateVideoGameCharacterDto
(
    [Required]
    [StringLength(50)]
    string Name,
    [Required]
    [StringLength(50)]
    string Game,
    [Required]
    [StringLength(50)]
    string Role
);