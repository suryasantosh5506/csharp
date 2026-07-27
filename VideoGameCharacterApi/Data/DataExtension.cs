using Microsoft.EntityFrameworkCore;
using VideoGameCharacterApi.Models;

namespace VideoGameCharacterApi.Data;

public static class DataExtension
{
    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<VideoGameCharacterContext>();

        dbContext.Database.Migrate();
    }

    public static void SeedDb(this WebApplicationBuilder builder)
    {
        const string connString = "Data Source=VideoGameCharacter.db";

        builder.Services.AddSqlite<VideoGameCharacterContext>(
            connString,
            optionsAction:(options)=>options.UseSeeding((context,_)=>
            {
                if (context.Set<VideoGameCharacter>().Any())
                    return;

                context.Set<VideoGameCharacter>().AddRange(
                    new VideoGameCharacter
                    {
                        Name = "Mario",
                        Game = "Super Mario Bros.",
                        Role = "Hero"
                    },
                    new VideoGameCharacter
                    {
                        Name = "Luigi",
                        Game = "Super Mario Bros.",
                        Role = "Hero"
                    },
                    new VideoGameCharacter
                    {
                        Name = "Princess Peach",
                        Game = "Super Mario Bros.",
                        Role = "Princess"
                    },
                    new VideoGameCharacter
                    {
                        Name = "Bowser",
                        Game = "Super Mario Bros.",
                        Role = "Villain"
                    },
                    new VideoGameCharacter
                    {
                        Name = "Link",
                        Game = "The Legend of Zelda",
                        Role = "Hero"
                    },
                    new VideoGameCharacter
                    {
                        Name = "Zelda",
                        Game = "The Legend of Zelda",
                        Role = "Princess"
                    },
                    new VideoGameCharacter
                    {
                        Name = "Ganondorf",
                        Game = "The Legend of Zelda",
                        Role = "Villain"
                    },
                    new VideoGameCharacter
                    {
                        Name = "Kratos",
                        Game = "God of War",
                        Role = "God Slayer"
                    },
                    new VideoGameCharacter
                    {
                        Name = "Master Chief",
                        Game = "Halo",
                        Role = "Soldier"
                    },
                    new VideoGameCharacter
                    {
                        Name = "Geralt of Rivia",
                        Game = "The Witcher",
                        Role = "Witcher"
                    }
                );

                context.SaveChanges();
            })
        );
    }
}