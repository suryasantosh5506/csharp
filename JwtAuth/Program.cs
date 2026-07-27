using System.Text;
using JwtAuth.Data;
using JwtAuth.Interfaces;
using JwtAuth.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Experimental;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
builder.SeedDb();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddControllers();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtToken:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtToken:Audience"],

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["JwtToken:SecretKey"]!
                )
            ),

            ClockSkew = TimeSpan.Zero
        };
    });


var app = builder.Build();

app.MigrateDb();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();