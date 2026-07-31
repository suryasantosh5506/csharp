using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementAPI.Middlewares;

public class ExceptionMiddleware(RequestDelegate next,ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }catch(Exception ex){
            logger.LogError(ex,ex.Message);

            context.Response.ContentType="application/json";
            context.Response.StatusCode=500;

            var response=new ProblemDetails
            {
                Title=ex.Message,
                Status=500
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}