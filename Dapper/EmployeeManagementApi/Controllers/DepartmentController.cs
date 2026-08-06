using EmployeeManagementApi.Dtos.Department;
using EmployeeManagementApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementApi.Controllers;

public class DepartmentController(IDepartmentService departmentService)
    : BaseApiController
{
    [HttpGet("company/{companyId:int}")]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments(int companyId)
    {
        var departments = await departmentService.GetAllDepartmentsAsync(companyId);
        return Ok(departments);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DepartmentDto>> GetDepartment(int id)
    {
        var department = await departmentService.GetDepartmentByIdAsync(id);
        return Ok(department);
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> CreateDepartment(CreateDepartmentDto dto)
    {
        var department = await departmentService.CreateDepartmentAsync(dto);

        return CreatedAtAction(
            nameof(GetDepartment),
            new { id = department.Id },
            department);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateDepartment(int id, UpdateDepartmentDto dto)
    {
        await departmentService.UpdateDepartmentAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteDepartment(int id)
    {
        await departmentService.DeleteDepartmentAsync(id);
        return NoContent();
    }
}