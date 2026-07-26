using LibraryManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Data;

public class LibraryManagementContext(DbContextOptions<LibraryManagementContext> options) : DbContext(options)
{
    public DbSet<Author>Authors=>Set<Author>();
    public DbSet<Book>Books=>Set<Book>();
    public DbSet<Category>Categories=>Set<Category>();
}