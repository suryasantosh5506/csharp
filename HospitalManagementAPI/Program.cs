using HospitalManagementAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddValidation();
builder.Services.AddControllers();
builder.AddDatabase();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MigrateDb();

app.MapGet("/", () => "Hello World!");

app.Run();
