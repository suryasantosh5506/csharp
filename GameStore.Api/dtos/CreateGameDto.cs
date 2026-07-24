namespace GameStore.Api.dtos;
public record CreateGameDto
(
    string Name,
    string Genre,
    decimal Price,
    DateOnly ReleaseDate
);