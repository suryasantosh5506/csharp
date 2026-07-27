using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.dtos;

public record class GameDetailsDto(
    int Id,
    [Required]
    [StringLength(50)]
    string Name,
    [Required]
    [Range(1,50)]
    int GenreId,
    [Range(1,100)]
    decimal Price,
    DateOnly ReleaseDate
);