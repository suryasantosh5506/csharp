using LearnHubApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearnHubApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category>Categories=>Set<Category>();
    public DbSet<Course>Courses=>Set<Course>();
    public DbSet<Enrollment>Enrollments=>Set<Enrollment>();
    public DbSet<Lesson>Lessons=>Set<Lesson>();
    public DbSet<Module>Modules=>Set<Module>();
    public DbSet<Review>Reviews=>Set<Review>();
    public DbSet<User>Users=>Set<User>();
}