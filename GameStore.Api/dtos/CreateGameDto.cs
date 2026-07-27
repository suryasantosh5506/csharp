using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.dtos;
public record CreateGameDto
(
    [Required]
    int Id,
    [Required]
    [StringLength(50)]
    string Name,
    [Required]
    [Range(1,50)]
    int GenreId,
    [Required]
    [Range(1,100)]
    decimal Price,
    [Required]
    DateOnly ReleaseDate
);