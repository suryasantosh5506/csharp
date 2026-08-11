using System.Net;
using System.Text.Json;
using JobManagementApi.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace JobManagementApi.Middlewares;

public class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context,ILogger<ExceptionMiddleware>logger)
    {
        try
        {
            await next(context);
        }catch(Exception ex){
            context.Response.ContentType="application/json";
            logger.LogDebug(ex,"An exception occured");

            int status=ex switch
            {
                BadRequestException=>(int)HttpStatusCode.BadRequest,
                ConflictException=>(int)HttpStatusCode.Conflict,
                ForbiddenException=>(int)HttpStatusCode.Forbidden,
                NotFoundException=>(int)HttpStatusCode.NotFound,
                UnauthorizedException=>(int)HttpStatusCode.Unauthorized,
                _=>(int)HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode=status;

            var response =new ProblemDetails
            {
                Status=status,
                Title=ex.Message
            };

            Console.WriteLine(ex.Message);

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}