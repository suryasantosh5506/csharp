using LibraryManagementAPI.Data;
using LibraryManagementAPI.Dtos.Category;
using LibraryManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Routes;
public static class CategoryApiRoutes
{
    public static void  MapCategoryApiRoutes(this WebApplication app)
    {
        var group=app.MapGroup("/Category");

        group.MapGet("/",async (LibraryManagementContext dbContext) =>
        {
           return await  dbContext.Categories.Select(category=>new CategoryDetailsDto(category.Id,category.Name)).ToListAsync();
        });

        group.MapGet("/{id}",async (int id,LibraryManagementContext dbContext) =>
        {
           var category=await dbContext.Categories.FindAsync(id);
           if(category is null) return Results.NotFound();
           return Results.Ok(new CategoryDetailsDto(category.Id,category.Name));
        }).WithName("GetCategoryById");

        group.MapPost("/",async (CreateCategoryDto newCategory,LibraryManagementContext dbContext) =>
        {
            if(await dbContext.Categories.AnyAsync(category => category.Name == newCategory.Name))
            {
                return Results.Conflict();
            }
            Category category = new()
            {
                Name=newCategory.Name
            };

            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();
            return Results.CreatedAtRoute("GetCategoryById",new{id=category.Id},new CategoryDetailsDto(category.Id,category.Name));
        });

        group.MapPut("/{id}",async(int id,UpdateCategoryDto updateCategory,LibraryManagementContext dbContext) =>
        {

            if (await dbContext.Categories.AnyAsync(c =>c.Name == updateCategory.Name && c.Id != id))
            {
                return Results.Conflict("Category already exists.");
            }

            var exist=await dbContext.Categories.FindAsync(id);
            if(exist is null) return Results.NotFound();
            exist.Name=updateCategory.Name;
            await dbContext.SaveChangesAsync();
            return Results.Ok(new CategoryDetailsDto(exist.Id,exist.Name));
        });

        group.MapDelete("/{id}",async(int id,LibraryManagementContext dbContext) =>
        {
           await dbContext.Books.Where(x=>x.CategoryId==id).ExecuteDeleteAsync();
           await dbContext.Categories.Where(x=>x.Id==id).ExecuteDeleteAsync();
           return Results.NoContent();
        });
    }
}