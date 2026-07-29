using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Data;

public static class DataExtension
{
    public static void MigrateDb(this WebApplication app)
    {
        using var scope=app.Services.CreateScope();
        var dbContext=scope.ServiceProvider.GetRequiredService<HospitalContext>();
        dbContext.Database.Migrate();
    }

    public static void AddDatabase(this WebApplicationBuilder builder)
    {
        var connString=builder.Configuration["ConnectionStrings:DefaultConnection"];
        builder.Services.AddSqlite<HospitalContext>(connString);
    }
}