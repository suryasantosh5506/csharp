using LibraryManagementAPI.Data;
using LibraryManagementAPI.Dtos.Author;
using LibraryManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Routes;

public static class AuthorApiRoutes
{
    public static void MapAuthorApiRoutes(this WebApplication app)
    {
        var group=app.MapGroup("/Author");

        group.MapGet("/",async(LibraryManagementContext dbContext) =>
        {
           return await dbContext.Authors.Select(author=>new AuthorDetailsDto(
            author.Id,
            author.Name,
            author.Email,
            author.Country
           ))
           .ToListAsync();
        });

        group.MapGet("/{id}",async(int id,LibraryManagementContext dbContext) =>
        {
            var existingAuthor=await dbContext.Authors.FindAsync(id);
            if(existingAuthor is null) return Results.NotFound();
            var author=new AuthorDetailsDto(
                existingAuthor.Id,
                existingAuthor.Name,
                existingAuthor.Email,
                existingAuthor.Country
            );
            return Results.Ok(author);
        }).WithName("GetAuthorById");

        group.MapPost("/",async (CreateAuthorDto newAuthor,LibraryManagementContext dbContext) =>
        {
            if (await dbContext.Authors.AnyAsync(a => a.Email == newAuthor.Email))
            {
                return Results.Conflict("Author with this email already exists.");
            }
           Author author = new()
           {
               Name=newAuthor.Name,
               Email=newAuthor.Email,
               Country=newAuthor.Country
           };

           dbContext.Authors.Add(author);
           await dbContext.SaveChangesAsync();
           return Results.CreatedAtRoute("GetAuthorById",new{id=author.Id},new AuthorDetailsDto(author.Id,author.Name,author.Email,author.Country));
        });

        group.MapPut("/{id}",async(int id,UpdateAuthorDto updateAuthor,LibraryManagementContext dbContext) =>
        {
            Author author=await dbContext.Authors.FindAsync(id);
            if(author is null) return Results.NotFound();
            author.Name=updateAuthor.Name;
            author.Email=updateAuthor.Email;
            author.Country=updateAuthor.Country;
            await dbContext.SaveChangesAsync();
            return Results.Ok(new AuthorDetailsDto(author.Id,author.Name,author.Email,author.Country));
        });

        group.MapDelete("/{id}",async(int id,LibraryManagementContext dbContext)=>{
            await dbContext.Books.Where(book=>book.AuthorId==id).ExecuteDeleteAsync();
            await dbContext.Authors.Where(author=>author.Id==id).ExecuteDeleteAsync();
            return Results.NoContent();
        });
    }
}