using GameStore.Api.dtos;

var builder=WebApplication.CreateBuilder(args);
var app=builder.Build();

List<GameDto> games = new()
{
    new(1,"Name1","Genre1",19.99m,new DateOnly(1992,7,15)),
    new(2,"Name2","Genre2",20.99m,new DateOnly(1992,7,16)),
    new(3,"Name3","Genre3",21.99m,new DateOnly(1992,7,17)),
    new(4,"Name4","Genre4",22.99m,new DateOnly(1992,7,18)),
    new(5,"Name5","Genre5",23.99m,new DateOnly(1992,7,19)),
};

app.MapGet("/", () => games);
app.MapGet("/games/{id}", (int id) =>
{
    return games.FirstOrDefault(game=>game.Id==id);
}).WithName("GetGame");

app.MapPost("/games",(CreateGameDto newGame) =>
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

app.MapPut("/games/{id}",(int id,UpdateGameDto updatedGame) =>
{
   int index=games.FindIndex(x=>x.Id==id);
   games[index]=new GameDto(
     games.Max(x=>x.Id),
     updatedGame.Name,
     updatedGame.Genre,
     updatedGame.Price,
     updatedGame.ReleaseDate
   );
   return Results.NoContent();
});


app.MapDelete("games/{id}",(int id) =>
{
    games.RemoveAll(x=>x.Id==id);
    return Results.NoContent();
});

app.Run();