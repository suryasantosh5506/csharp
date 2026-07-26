using Microsoft.AspNetCore.Components.RenderTree;
using VideoGameCharacterApi.Data;
using VideoGameCharacterApi.Interfaces;
using VideoGameCharacterApi.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<IVideoGameCharacterService,VideoGameCharacterService>();
builder.Services.AddControllers();
builder.Services.AddValidation();
builder.SeedDb();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapControllers();
app.MigrateDb();
app.Run();
