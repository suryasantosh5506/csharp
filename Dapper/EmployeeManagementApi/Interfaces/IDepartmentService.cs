using EmployeeManagementApi.Dtos.Department;
using EmployeeManagementApi.RequestHelpers.Pagination;

namespace EmployeeManagementApi.Interfaces;

public interface IDepartmentService
{
    Task<PagedList<DepartmentDto>> GetAllDepartmentsAsync(int companyId,PaginationParams paginationParams);

    Task<DepartmentDto?> GetDepartmentByIdAsync(int id);

    Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto dto);

    Task<bool> UpdateDepartmentAsync(int id, UpdateDepartmentDto dto);

    Task<bool> DeleteDepartmentAsync(int id);

    Task<DepartmentDetailsDto> GetDepartmentDetailsAsync(int id);
}