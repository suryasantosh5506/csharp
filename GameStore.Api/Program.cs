using GameStore.Api.Routes;

var builder=WebApplication.CreateBuilder(args);
var app=builder.Build();


app.MapGameApiRoutes();

app.Run();