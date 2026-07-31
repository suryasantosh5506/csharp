using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Department;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.Extensions;
using HospitalManagementAPI.Interfaces;
using HospitalManagementAPI.RequestHelpers;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Services;

public class DepartmentService(HospitalContext context) : IDepartmentService
{
    public async Task<PagedList<DepartmentDetailsDto>> GetAllDepartmentsAsync(PaginationParams paginationParams)
    {
        var query=context.Departments.Select(x=>x.ToDto());

        var departments=await PagedList<DepartmentDetailsDto>.ToPagedList(
            query,
            paginationParams.pageNumber,
            paginationParams.pageSize);

        return departments;
    }

    public async Task<DepartmentDetailsDto?> GetDepartmentByIdAsync(int id)
    {
        var department=await context.Departments.FindAsync(id);

        if(department is null) return null;

        return department.ToDto();
    }

    public async Task<DepartmentDetailsDto> CreateDepartmentAsync(CreateDepartmentDto createDepartmentDto)
    {
        Department department=new()
        {
            Name=createDepartmentDto.Name.Trim(),
            Description=createDepartmentDto.Description.Trim()
        };

        context.Departments.Add(department);

        await context.SaveChangesAsync();

        return department.ToDto();
    }

    public async Task<DepartmentDetailsDto?> UpdateDepartmentAsync(int id,UpdateDepartmentDto updateDepartmentDto)
    {
        var department=await context.Departments.FindAsync(id);

        if(department is null) return null;

        department.Name=updateDepartmentDto.Name.Trim();
        department.Description=updateDepartmentDto.Description.Trim();

        await context.SaveChangesAsync();

        return department.ToDto();
    }

    public async Task<bool> DeleteDepartmentAsync(int id)
    {
        var department=await context.Departments.FindAsync(id);

        if(department is null) return false;

        context.Departments.Remove(department);

        await context.SaveChangesAsync();

        return true;
    }
}