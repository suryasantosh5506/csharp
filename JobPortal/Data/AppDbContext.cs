using JobPortal.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Application> Applications=>Set<Application>();
    public DbSet<Company> Companies=>Set<Company>();
    public DbSet<Job> Jobs=>Set<Job>();
    public DbSet<User> Users=>Set<User>();
}