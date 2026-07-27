using System.Security.Cryptography.X509Certificates;
using GameStore.Api.Data;
using GameStore.Api.dtos;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Routes;

public static class GenreApiRoutes{
    public static void MapGenreApiRoutes(this WebApplication app)
    {
        var group=app.MapGroup("/genre");
        group.MapGet("/",async (GameStoreContext dbContext) =>
        {
            return await dbContext.Genres
                        .Select(genre=>new GenreDto(genre.Id,genre.Name))
                        .ToListAsync();
                
        });
    }
}