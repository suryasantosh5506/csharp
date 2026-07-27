namespace GameStore.Api.Routes;

using GameStore.Api.Data;
using GameStore.Api.dtos;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

public static class GameApiRoutes{
    public static void MapGameApiRoutes(this WebApplication app)
    {

        var group=app.MapGroup("/games");

        group.MapGet("/", async (GameStoreContext dbContext) =>
        {
            return await dbContext.Games
                .Include(game=>game.Genre)
                .Select(game=>new GameDto(
                    game.Id,
                    game.Name,
                    game.Genre!.Name,
                    game.Price,
                    game.ReleaseDate
                ))
                .ToListAsync();
            
        });
        group.MapGet("/{id}", async (int id,GameStoreContext dbContext) =>
        {
            var game=await dbContext.Games.FindAsync(id);
            if(game is null) return Results.NotFound();
            GameDetailsDto gameDetailsDto=new(
                game.Id,
                game.Name,
                game.GenreId,
                game.Price,
                game.ReleaseDate
            );
            return Results.Ok(gameDetailsDto);
        }).WithName("GetGame");

        group.MapPost("/",async (CreateGameDto newGame,GameStoreContext dbContext) =>
        {

            Game game = new()
            {
                Name=newGame.Name,
                GenreId=newGame.GenreId,
                Price=newGame.Price,
                ReleaseDate=newGame.ReleaseDate
            };
            
            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            GameDetailsDto createdGame = new(game.Id,
                game.Name,
                game.GenreId,
                game.Price,
                game.ReleaseDate
            );
            return Results.CreatedAtRoute("GetGame",new {id=createdGame.Id},createdGame);
        });

        group.MapPut("/{id}",async (int id,UpdateGameDto updatedGame,GameStoreContext dbContext) =>
        {
            Game existingGame=await dbContext.Games.FindAsync(id);
            if(existingGame is null) return Results.NotFound();
            existingGame.Name=updatedGame.Name;
            existingGame.GenreId=updatedGame.GenreId;
            existingGame.Price=updatedGame.Price;
            existingGame.ReleaseDate=updatedGame.ReleaseDate;

            await dbContext.SaveChangesAsync();

           GameDetailsDto game = new(
                existingGame.Id,
                existingGame.Name,
                existingGame.GenreId,
                existingGame.Price,
                existingGame.ReleaseDate
            );
            return Results.Ok(game);
        });


        group.MapDelete("/{id}",async (int id,GameStoreContext dbContext) =>
        {
            await dbContext.Games.Where(x=>x.Id==id).ExecuteDeleteAsync();
            return Results.NoContent();
        });

    }
}