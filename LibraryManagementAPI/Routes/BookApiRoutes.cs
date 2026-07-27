using LibraryManagementAPI.Data;
using LibraryManagementAPI.Dtos.Book;
using LibraryManagementAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Routes;
public static class BookApiRoutes
{
    public static void MapBookApiRoutes(this WebApplication app)
    {
        var group=app.MapGroup("/Books");

        group.MapGet("/",async(LibraryManagementContext dbContext) =>
        {
           return await dbContext.Books
                                    .Include(book=>book.Author)
                                    .Include(book=>book.Category)
                                    .Select(book=>new BookDetailsDto(
                                                    book.Id,
                                                    book.Title,
                                                    book.Price,
                                                    book.PublishedDate,
                                                    book.Stock,
                                                    book.Author.Name,
                                                    book.Category.Name
                                                ))
                                    .ToListAsync();
        });

        group.MapGet("/{id:int}",async (int id,LibraryManagementContext dbContext) =>
        {
           var book=await dbContext.Books.Include(book => book.Author)
                                         .Include(book => book.Category)
                                         .FirstOrDefaultAsync(book => book.Id == id);

            if(book is null) return Results.NotFound();
            return Results.Ok(new BookDetailsDto(
                                book.Id,
                                book.Title,
                                book.Price,
                                book.PublishedDate,
                                book.Stock,
                                book.Author.Name,
                                book.Category.Name
                    ));
        }).WithName("GetBookById");

        group.MapGet("/{title}",async (string title,LibraryManagementContext dbContext) =>
        {
           var book=await dbContext.Books
                                        .Include(x=>x.Author)
                                        .Include(x=>x.Category)
                                        .FirstOrDefaultAsync(x=>x.Title==title);

            if(book is null) return Results.NotFound("Book with Title Not Found");
            return Results.Ok(new BookDetailsDto(
                                book.Id,
                                book.Title,
                                book.Price,
                                book.PublishedDate,
                                book.Stock,
                                book.Author.Name,
                                book.Category.Name
                    ));
        });

        group.MapGet("/author/{id}",async (int id,LibraryManagementContext dbContext) =>
        {
           return await dbContext.Books
                          .Include(book=>book.Author)
                          .Include(book=>book.Category)
                          .Where(book=>book.AuthorId==id)
                          .Select(book=>new BookDetailsDto(
                            book.Id,
                            book.Title,
                            book.Price,
                            book.PublishedDate,
                            book.Stock,
                            book.Author.Name,
                            book.Category.Name
                          ))
                          .ToListAsync();
        });

        group.MapGet("/category/{id}",async (int id,LibraryManagementContext dbContext) =>
        {
           return await dbContext.Books
                          .Include(book=>book.Author)
                          .Include(book=>book.Category)
                          .Where(book=>book.CategoryId==id)
                          .Select(book=>new BookDetailsDto(
                            book.Id,
                            book.Title,
                            book.Price,
                            book.PublishedDate,
                            book.Stock,
                            book.Author.Name,
                            book.Category.Name
                          ))
                          .ToListAsync();
        });


        group.MapPost("/", async (CreateBookDto newBook, LibraryManagementContext dbContext) =>
        {
            if (!await dbContext.Authors.AnyAsync(a => a.Id == newBook.AuthorId))
            {
                return Results.BadRequest("Invalid AuthorId.");
            }

            if (!await dbContext.Categories.AnyAsync(c => c.Id == newBook.CategoryId))
            {
                return Results.BadRequest("Invalid CategoryId.");
            }

            Book book = new()
            {
                Title = newBook.Title,
                Price = newBook.Price,
                PublishedDate = newBook.PublishedDate,
                Stock = newBook.Stock,
                AuthorId = newBook.AuthorId,
                CategoryId = newBook.CategoryId
            };

            dbContext.Books.Add(book);
            await dbContext.SaveChangesAsync();

            book = await dbContext.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstAsync(b => b.Id == book.Id);

            return Results.CreatedAtRoute(
                "GetBookById",
                new { id = book.Id },
                new BookDetailsDto(
                    book.Id,
                    book.Title,
                    book.Price,
                    book.PublishedDate,
                    book.Stock,
                    book.Author.Name,
                    book.Category.Name
                ));
        });

        group.MapPut("/{id}", async (
                    int id,
                    UpdateBookDto updateBook,
                    LibraryManagementContext dbContext) =>
        {
            var book = await dbContext.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book is null)
                return Results.NotFound();

            if (!await dbContext.Authors.AnyAsync(a => a.Id == updateBook.AuthorId))
            {
                return Results.BadRequest("Invalid AuthorId.");
            }

            if (!await dbContext.Categories.AnyAsync(c => c.Id == updateBook.CategoryId))
            {
                return Results.BadRequest("Invalid CategoryId.");
            }

            book.Title = updateBook.Title;
            book.Price = updateBook.Price;
            book.PublishedDate = updateBook.PublishedDate;
            book.Stock = updateBook.Stock;
            book.AuthorId = updateBook.AuthorId;
            book.CategoryId = updateBook.CategoryId;

            await dbContext.SaveChangesAsync();

            // Reload to refresh navigation properties
            book = await dbContext.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstAsync(b => b.Id == id);

            return Results.Ok(
                new BookDetailsDto(
                    book.Id,
                    book.Title,
                    book.Price,
                    book.PublishedDate,
                    book.Stock,
                    book.Author.Name,
                    book.Category.Name
                ));
        });

        group.MapDelete("/{id}",async(int id,LibraryManagementContext dbContext) =>
        {
            await dbContext.Books.Where(x=>x.Id==id).ExecuteDeleteAsync();
            return Results.NoContent();
        });
    }
}