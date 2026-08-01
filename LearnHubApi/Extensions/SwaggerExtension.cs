using Microsoft.OpenApi;

namespace LearnHubApi.Extensions;

public static class SwaggerExtensions
{
    public static void ConfigureSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "LearnHub API",
                Version = "v1"
            });
        });
    }

    public static void UseSwaggerDocumentation(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}