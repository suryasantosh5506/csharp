using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Models;
namespace StudentManagementAPI.Data;

public class StudentManagementContext(DbContextOptions<StudentManagementContext>options) : DbContext(options)
{
    public DbSet<Student>students=>Set<Student>();
    public DbSet<Department>Departments=>Set<Department>();
}