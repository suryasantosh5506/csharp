namespace  StudentManagementAPI.Routes;

using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Data;
using StudentManagementAPI.dtos.Departments;
using StudentManagementAPI.Models;

public static class DepartmentApiRoutes
{
    public static void MapDepartmentApiRoutes(this WebApplication app)
    {
        var group=app.MapGroup("/departments");

        group.MapGet("/",async(StudentManagementContext dbContext) =>
        {
           return await dbContext.Departments
            .Select(dept => new DepartmentDetailsDto(
                dept.Name
            ))
            .ToListAsync();
        });


        group.MapGet("/{id}",async(int id,StudentManagementContext dbContext) =>
        {
            var dept=await dbContext.Departments.FindAsync(id);
            if(dept is null) return Results.NotFound();
            return Results.Ok(
                new DepartmentDetailsDto(dept.Name)
            );
        }).WithName("GetDepartment");

        group.MapPost("/",async (CreateDepartmentDto newDepartment,StudentManagementContext dbContext) =>
        {

            if (await dbContext.Departments
                .AnyAsync(d => d.Name == newDepartment.Name))
            {
                return Results.Conflict("Department already exists.");
            }

           Department dept=new(){
                Name=newDepartment.Name
            };
            dbContext.Departments.Add(dept);
            await dbContext.SaveChangesAsync();
            return Results.CreatedAtRoute("GetDepartment",new{id=dept.Id},new DepartmentDetailsDto(dept.Name));
        });


        group.MapPut("/{id}",async(int id,UpdateDepartmentDto updateDept,StudentManagementContext dbContext) =>
        {
            if (await dbContext.Departments
                .AnyAsync(d => d.Name == updateDept.Name  && d.Id != id))
            {
                return Results.Conflict("Department already exists.");
            }
            var dept=await dbContext.Departments.FindAsync(id);
            if(dept is null) return Results.NotFound();
            dept.Name=updateDept.Name;
            await dbContext.SaveChangesAsync();
            return Results.Ok(new DepartmentDetailsDto(dept.Name));
        });

        group.MapDelete("/{id}",async(int id,StudentManagementContext dbContext) =>
        {
           await dbContext.students.Where(student=>student.DepartmentId==id).ExecuteDeleteAsync();
           await dbContext.Departments.Where(dept=>dept.Id==id).ExecuteDeleteAsync();
           return Results.NoContent(); 
        });
    }
}