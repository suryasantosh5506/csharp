using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.dtos;

public record GenreDto
(
    [Required]
    [Range(1,50)]
    int Id,
    [Required]
    [StringLength(50)]
    string Name
);