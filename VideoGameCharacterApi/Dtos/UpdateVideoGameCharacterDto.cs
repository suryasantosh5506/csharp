using System.ComponentModel.DataAnnotations;

namespace VideoGameCharacterApi.Dtos;

public record UpdateVideoGameCharacterDto
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