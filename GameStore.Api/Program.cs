using GameStore.Api.Data;
using GameStore.Api.Routes;

var builder=WebApplication.CreateBuilder(args);

builder.Services.AddValidation();
var connString="Data Source=GameStore.db";
builder.Services.AddSqlite<GameStoreContext>(connString);

var app=builder.Build();


app.MapGameApiRoutes();

app.Run();