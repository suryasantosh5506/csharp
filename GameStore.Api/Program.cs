using GameStore.Api.Data;
using GameStore.Api.Models;
using GameStore.Api.Routes;

var builder=WebApplication.CreateBuilder(args);

builder.Services.AddValidation();
builder.SeedDb();

var app=builder.Build();

app.MigrateDb();
app.MapGameApiRoutes();
app.MapGenreApiRoutes();

app.Run();