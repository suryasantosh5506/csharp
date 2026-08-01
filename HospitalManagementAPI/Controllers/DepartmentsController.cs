using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Department;
using HospitalManagementAPI.enums;
using HospitalManagementAPI.Interfaces;
using HospitalManagementAPI.RequestHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Controllers;

[Authorize(Roles =nameof(UserRole.Admin))]
public class DepartmentsController(HospitalContext context,IDepartmentService departmentService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PagedList<DepartmentDetailsDto>>> GetAllDepartmentsAsync([FromQuery]PaginationParams paginationParams)
    {
        var departments=await departmentService.GetAllDepartmentsAsync(paginationParams);

        return Ok(departments);
    }

    [HttpGet("{id}",Name ="GetDepartmentById")]
    public async Task<ActionResult<DepartmentDetailsDto>> GetDepartmentByIdAsync(int id)
    {
        var department=await departmentService.GetDepartmentByIdAsync(id);

        if(department is null) return NotFound();

        return Ok(department);
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentDetailsDto>> CreateDepartmentAsync(CreateDepartmentDto newDepartment)
    {
        if(await context.Departments.AnyAsync(d=>
            d.Name.ToLower()==newDepartment.Name.Trim().ToLower()))
        {
            return Conflict();
        }

        var department=await departmentService.CreateDepartmentAsync(newDepartment);

        return CreatedAtRoute("GetDepartmentById",new{id=department.Id},department);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<DepartmentDetailsDto>> UpdateDepartmentAsync(int id,UpdateDepartmentDto updateDepartmentDto)
    {
        if(await context.Departments.AnyAsync(x=>
            x.Name.ToLower()==updateDepartmentDto.Name.Trim().ToLower() &&
            x.Id!=id))
        {
            return Conflict();
        }

        var department=await departmentService.UpdateDepartmentAsync(id,updateDepartmentDto);

        if(department is null) return NotFound();

        return Ok(department);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteDepartmentAsync(int id)
    {
        var deleted=await departmentService.DeleteDepartmentAsync(id);

        if(!deleted) return NotFound();

        return NoContent();
    }
}