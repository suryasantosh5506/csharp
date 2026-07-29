using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Department;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Controllers;

public class DepartmentsController(HospitalContext context) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<DepartmentDetailsDto>>> GetAllDepartmentsAsync()
    {
        var departments = await context.Departments
        .Select(x => x.ToDto())
        .ToListAsync();

        return Ok(departments);
    }

    [HttpGet("{id}",Name ="GetDepartmentById")]
    public async Task<ActionResult<DepartmentDetailsDto>> GetDepartmentByIdAsync(int id)
    {
        var department=await context.Departments.FindAsync(id);
        if(department is null) return NotFound();
        return Ok(department.ToDto());
    }

    [HttpPost]

    public async Task<ActionResult<DepartmentDetailsDto>> CreateDepartmentAsync(CreateDepartmentDto newDepartment)
    {
        if(await context.Departments.AnyAsync(d =>
                d.Name.ToLower() == newDepartment.Name.Trim().ToLower())) return Conflict();
        var department=new Department()
        {
            Name=newDepartment.Name,
            Description=newDepartment.Description
        };
        context.Departments.Add(department);
        await context.SaveChangesAsync();
        return CreatedAtRoute("GetDepartmentById",new {id=department.Id},department.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<DepartmentDetailsDto>> UpdateDepartmentAsync(int id,UpdateDepartmentDto updateDepartmentDto)
    {
        var department=await context.Departments.FindAsync(id);

        if(department is null) return NotFound();

        if(await context.Departments.AnyAsync(x => x.Name.ToLower() == updateDepartmentDto.Name.Trim().ToLower() && x.Id!=id))
        {
            return Conflict();
        }
        
        department.Name=updateDepartmentDto.Name.Trim();
        department.Description=updateDepartmentDto.Description.Trim();
        await context.SaveChangesAsync();
        return Ok(department.ToDto());
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteDepartmentAsync(int id)
    {
        var department=await context.Departments.FindAsync(id);
        if(department is null) return NotFound();
        context.Departments.Remove(department);
        await context.SaveChangesAsync();
        return NoContent();
    }
}