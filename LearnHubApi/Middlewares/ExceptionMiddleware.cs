using System.Text.Json;
using LearnHubApi.Exceptions;

namespace LearnHubApi.Middlewares;

public class ExceptionMiddleware(RequestDelegate _next,ILogger<ExceptionMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }catch(Exception ex)
        {
            logger.LogError(ex,ex.Message);
            var statusCode=ex switch
            {
                NotFoundException=>StatusCodes.Status404NotFound,
                UnauthorizedException=>StatusCodes.Status401Unauthorized,
                ForbiddenException => StatusCodes.Status403Forbidden,
                ConflictException => StatusCodes.Status409Conflict,
                BadRequestException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";


            var response =new
            {
                Error=ex.Message,
                Status=statusCode
            };
            var jsonres=JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonres);
        }
    }
}