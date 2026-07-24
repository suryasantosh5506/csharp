namespace GameStore.Api.Routes;
using GameStore.Api.dtos;
public static class GameApiRoutes{
    private static List<GameDto> games = new()
    {

        new(1,"Name1","Genre1",19.99m,new DateOnly(1992,7,15)),
        new(2,"Name2","Genre2",20.99m,new DateOnly(1992,7,16)),
        new(3,"Name3","Genre3",21.99m,new DateOnly(1992,7,17)),
        new(4,"Name4","Genre4",22.99m,new DateOnly(1992,7,18)),
        new(5,"Name5","Genre5",23.99m,new DateOnly(1992,7,19)),
    };

    public static void MapGameApiRoutes(this WebApplication app)
    {

        var group=app.MapGroup("/group");

        group.MapGet("/", () => games);
        group.MapGet("/{id}", (int id) =>
        {
            var game=games.FirstOrDefault(game=>game.Id==id);
            return (game is null)?Results.NotFound():Results.Ok(game);
        }).WithName("GetGame");

        group.MapPost("/",(CreateGameDto newGame) =>
        {
            GameDto game=new(
                games.Max(x=>x.Id),
                newGame.Name,
                newGame.Genre,
                newGame.Price,
                newGame.ReleaseDate
            );
            
            games.Add(game);
            return Results.CreatedAtRoute("GetGame",new {id=game.Id},game);
        });

        group.MapPut("/{id}",(int id,UpdateGameDto updatedGame) =>
        {
            int index=games.FindIndex(x=>x.Id==id);

            if (index == -1)
            {
                return Results.NotFound();
            }

            games[index]=new GameDto(
                id,
                updatedGame.Name,
                updatedGame.Genre,
                updatedGame.Price,
                updatedGame.ReleaseDate
            );
            return Results.NoContent();
        });


        group.MapDelete("/{id}",(int id) =>
        {
            games.RemoveAll(x=>x.Id==id);
            return Results.NoContent();
        });

    }
}