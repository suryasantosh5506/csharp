using LibraryManagementAPI.Data;
using LibraryManagementAPI.Routes;

var builder = WebApplication.CreateBuilder(args);
builder.SeedDb();
var app = builder.Build();

app.MigrateDB();


app.MapGet("/", () => "Hello World!");
app.MapAuthorApiRoutes();
app.MapCategoryApiRoutes();
app.MapBookApiRoutes();

app.Run();
