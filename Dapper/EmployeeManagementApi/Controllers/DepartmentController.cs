using EmployeeManagementApi.Dtos.Department;
using EmployeeManagementApi.Interfaces;
using EmployeeManagementApi.RequestHelpers.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementApi.Controllers;

public class DepartmentController(IDepartmentService departmentService)
    : BaseApiController
{
    [HttpGet("company/{companyId:int}")]
    public async Task<ActionResult<PagedList<DepartmentDto>>> GetDepartments(int companyId,[FromQuery]PaginationParams paginationParams)
    {
        var departments = await departmentService.GetAllDepartmentsAsync(companyId,paginationParams);
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

    [HttpGet("{id:int}/details")]
    public async Task<ActionResult<DepartmentDetailsDto>> GetDepartmentDetails(int id)
    {
        var department = await departmentService.GetDepartmentDetailsAsync(id);
        return Ok(department);
    }
}