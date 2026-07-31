using HospitalManagementAPI.Dtos.Department;
using HospitalManagementAPI.RequestHelpers;

namespace HospitalManagementAPI.Interfaces;

public interface IDepartmentService
{
    Task<PagedList<DepartmentDetailsDto>> GetAllDepartmentsAsync(PaginationParams paginationParams);

    Task<DepartmentDetailsDto?> GetDepartmentByIdAsync(int id);

    Task<DepartmentDetailsDto> CreateDepartmentAsync(CreateDepartmentDto createDepartmentDto);

    Task<DepartmentDetailsDto?> UpdateDepartmentAsync(int id,UpdateDepartmentDto updateDepartmentDto);

    Task<bool> DeleteDepartmentAsync(int id);
}