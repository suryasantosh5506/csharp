using LearnHubApi.Data;
using LearnHubApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureDb();
builder.ConfigureSwagger();

builder.Services.AddControllers();

var app = builder.Build();

app.MigrateDb();

app.UseHttpsRedirection();

app.UseSwaggerDocumentation();

app.MapControllers();

app.Run();