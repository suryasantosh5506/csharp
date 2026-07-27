namespace StudentManagementAPI.Routes;

using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Data;
using StudentManagementAPI.dtos.Students;
using StudentManagementAPI.Models;

public static class StudentApiRoutes
{
    public static void MapStudentApiRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/students");

        // GET ALL
        group.MapGet("/", async (StudentManagementContext dbContext) =>
        {
            return await dbContext.students
                .Include(student => student.Department)
                .Select(student => new StudentDetailsDto(
                    student.Id,
                    student.FirstName,
                    student.LastName,
                    student.Email,
                    student.Age,
                    student.Department!.Name,
                    student.EnrollmentDate
                ))
                .ToListAsync();
        });

        // GET BY ID
        group.MapGet("/{id}", async (int id, StudentManagementContext dbContext) =>
        {
            var existingStudent = await dbContext.students
                .Include(student => student.Department)
                .FirstOrDefaultAsync(student => student.Id == id);

            if (existingStudent is null)
            {
                return Results.NotFound();
            }

            var stu = new StudentDetailsDto(
                existingStudent.Id,
                existingStudent.FirstName,
                existingStudent.LastName,
                existingStudent.Email,
                existingStudent.Age,
                existingStudent.Department!.Name,
                existingStudent.EnrollmentDate
            );

            return Results.Ok(stu);
        })
        .WithName("GetStudent");

        // POST
        group.MapPost("/", async (CreateStudentDto newStudent, StudentManagementContext dbContext) =>
        {
            Student student = new()
            {
                FirstName = newStudent.FirstName,
                LastName = newStudent.LastName,
                Email = newStudent.Email,
                Age = newStudent.Age,
                DepartmentId = newStudent.DepartmentId,
                EnrollmentDate = newStudent.EnrollmentDate
            };

            dbContext.students.Add(student);
            await dbContext.SaveChangesAsync();

            // Reload with Department
            student = await dbContext.students
                .Include(s => s.Department)
                .FirstAsync(s => s.Id == student.Id);

            var stu = new StudentDetailsDto(
                student.Id,
                student.FirstName,
                student.LastName,
                student.Email,
                student.Age,
                student.Department!.Name,
                student.EnrollmentDate
            );

            return Results.CreatedAtRoute(
                "GetStudent",
                new { id = student.Id },
                stu
            );
        });

        group.MapDelete("/{id}",async (int id,StudentManagementContext dbContext) =>
        {
            return await dbContext.students.Where(student=>student.Id==id).ExecuteDeleteAsync(); 
        });
    }
}