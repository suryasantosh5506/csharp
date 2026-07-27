using LibraryManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Data;

public static class DataExtensions
{
    public static void MigrateDB(this WebApplication app)
    {
        var scope=app.Services.CreateScope();
        var dbContext=scope.ServiceProvider.GetRequiredService<LibraryManagementContext>();
        dbContext.Database.Migrate();
    }

   public static void SeedDb(this WebApplicationBuilder builder)
    {
        var connString = "Data Source=LibraryManagement.db";

        builder.Services.AddSqlite<LibraryManagementContext>(
            connString,
            optionsAction: options => options.UseSeeding((context, _) =>
            {
                if (context.Set<Author>().Any())
                    return;


                context.Set<Author>().AddRange(
                    new Author
                    {
                        Name = "Robert C. Martin",
                        Email = "unclebob@gmail.com",
                        Country = "USA"
                    },
                    new Author
                    {
                        Name = "Martin Fowler",
                        Email = "martin.fowler@gmail.com",
                        Country = "UK"
                    },
                    new Author
                    {
                        Name = "Eric Evans",
                        Email = "eric.evans@gmail.com",
                        Country = "USA"
                    }
                );


                context.Set<Category>().AddRange(
                    new Category { Name = "Programming" },
                    new Category { Name = "Software Engineering" },
                    new Category { Name = "Architecture" }
                );

                context.SaveChanges();


                context.Set<Book>().AddRange(
                    new Book
                    {
                        Title = "Clean Code",
                        Price = 499.99m,
                        PublishedDate = new DateOnly(2008, 8, 1),
                        Stock = 20,
                        AuthorId = 1,
                        CategoryId = 1
                    },
                    new Book
                    {
                        Title = "Clean Architecture",
                        Price = 599.99m,
                        PublishedDate = new DateOnly(2017, 9, 20),
                        Stock = 15,
                        AuthorId = 1,
                        CategoryId = 3
                    },
                    new Book
                    {
                        Title = "Refactoring",
                        Price = 699.99m,
                        PublishedDate = new DateOnly(2018, 11, 20),
                        Stock = 12,
                        AuthorId = 2,
                        CategoryId = 2
                    },
                    new Book
                    {
                        Title = "Domain-Driven Design",
                        Price = 799.99m,
                        PublishedDate = new DateOnly(2003, 8, 30),
                        Stock = 10,
                        AuthorId = 3,
                        CategoryId = 3
                    }
                );

                context.SaveChanges();
            }));
    }
}