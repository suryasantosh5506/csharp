using StudentManagementAPI.Data;
using StudentManagementAPI.Routes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation();
builder.SeedDb();
var app = builder.Build();
app.MigrateDb();
app.MapGet("/", () => "Hello World!");
app.MapStudentApiRoutes();
app.MapDepartmentApiRoutes();

app.Run();
