using Microsoft.EntityFrameworkCore;

namespace JwtAuth.Data;

public static class DataExtension
{
    public static void MigrateDb(this WebApplication app)
    {
        var scope=app.Services.CreateScope();
        var dbContext=scope.ServiceProvider.GetRequiredService<JwtAuthContext>();
        dbContext.Database.Migrate();
    }

    public static void SeedDb(this WebApplicationBuilder builder)
    {
        var connString="Data Source=JwtAuth.db";
        builder.Services.AddSqlite<JwtAuthContext>(
            connectionString:connString
        );
    }
}