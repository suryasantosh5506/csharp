using Microsoft.EntityFrameworkCore;

namespace JobPortal.Data;

public static class DataExtension
{
    public static void MigrateDb(this WebApplication app)
    {
        var scope=app.Services.CreateScope();
        var context=scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }

    public static void ConfigureDb(this WebApplicationBuilder builder)
    {
        var connString=builder.Configuration["ConnectionStrings:DefaultConnection"];
        builder.Services.AddSqlite<AppDbContext>(connectionString:connString);
    }
}