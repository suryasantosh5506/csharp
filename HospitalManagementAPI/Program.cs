using System.Text;
using HospitalManagementAPI.Data;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.Interfaces;
using HospitalManagementAPI.Middlewares;
using HospitalManagementAPI.Options;
using HospitalManagementAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;



var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("JwtConfiguration"));


builder.Services.AddScoped<IDoctorService,DoctorService>();
builder.Services.AddScoped<IPatientService,PatientService>();
builder.Services.AddScoped<IAdminAppointmentService,AdminAppointmentService>();
builder.Services.AddScoped<IAdminDoctorApplicationService,AdminDoctorApplicationService>();
builder.Services.AddScoped<IAppointmentService,AppointmentService>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<IDepartmentService,DepartmentService>();
builder.Services.AddScoped<IDoctorApplicationService,DoctorApplicationService>();
builder.Services.AddScoped<IDoctorAppointmentService,DoctorAppointmentService>();
builder.Services.AddScoped<IPatientProfileService,PatientProfileService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddValidation();
builder.Services.AddControllers();
builder.Services.AddScoped<PasswordHasher<User>>();
builder.AddDatabase();

var jwtConfiguration = builder.Configuration
    .GetSection("JwtConfiguration")
    .Get<JwtConfiguration>()!;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtConfiguration.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtConfiguration.Audience,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtConfiguration.SecretKey))
        };
    });
builder.Services.AddAuthorization();


var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MigrateDb();
app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
