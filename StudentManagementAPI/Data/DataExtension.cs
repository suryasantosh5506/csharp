namespace StudentManagementAPI.Data;

using StudentManagementAPI.Models;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
public static class DataExtension
{
    public static void MigrateDb(this WebApplication app)
    {
        var scope=app.Services.CreateScope();
        var dbContext=scope.ServiceProvider.GetRequiredService<StudentManagementContext>();
        dbContext.Database.Migrate();
    }

    public static void SeedDb(this WebApplicationBuilder builder)
    {
        var connString="Data Source=StudentManagement.db";
        builder.Services.AddSqlite<StudentManagementContext>(
            connString,
            optionsAction: (options) => options.UseSeeding((context, _) =>
            {
                if (!context.Set<Department>().Any() && !context.Set<Student>().Any())
                {
                    context.Set<Department>().AddRange(
                        new Department
                        {
                            Id = 1,
                            Name = "Computer Science"
                        },
                        new Department
                        {
                            Id = 2,
                            Name = "Mechanical Engineering"
                        },
                        new Department
                        {
                            Id = 3,
                            Name = "Civil Engineering"
                        },
                        new Department
                        {
                            Id = 4,
                            Name = "Electrical Engineering"
                        }
                    );

                    context.SaveChanges();

                    context.Set<Student>().AddRange(
                        new Student
                        {
                            FirstName = "Rahul",
                            LastName = "Sharma",
                            Email = "rahul.sharma@example.com",
                            Age = 20,
                            DepartmentId = 1,
                            EnrollmentDate = new DateOnly(2024, 8, 1)
                        },
                        new Student
                        {
                            FirstName = "Priya",
                            LastName = "Reddy",
                            Email = "priya.reddy@example.com",
                            Age = 21,
                            DepartmentId = 1,
                            EnrollmentDate = new DateOnly(2023, 8, 1)
                        },
                        new Student
                        {
                            FirstName = "Arjun",
                            LastName = "Patel",
                            Email = "arjun.patel@example.com",
                            Age = 22,
                            DepartmentId = 2,
                            EnrollmentDate = new DateOnly(2022, 8, 1)
                        },
                        new Student
                        {
                            FirstName = "Sneha",
                            LastName = "Nair",
                            Email = "sneha.nair@example.com",
                            Age = 19,
                            DepartmentId = 3,
                            EnrollmentDate = new DateOnly(2024, 8, 1)
                        },
                        new Student
                        {
                            FirstName = "Kiran",
                            LastName = "Verma",
                            Email = "kiran.verma@example.com",
                            Age = 23,
                            DepartmentId = 4,
                            EnrollmentDate = new DateOnly(2021, 8, 1)
                        }
                    );

                    context.SaveChanges();
                }
            })
        );
    }
}