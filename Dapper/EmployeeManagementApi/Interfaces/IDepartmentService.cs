using EmployeeManagementApi.Dtos.Department;

namespace EmployeeManagementApi.Interfaces;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync(int companyId);

    Task<DepartmentDto?> GetDepartmentByIdAsync(int id);

    Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto dto);

    Task<bool> UpdateDepartmentAsync(int id, UpdateDepartmentDto dto);

    Task<bool> DeleteDepartmentAsync(int id);

    Task<DepartmentDetailsDto> GetDepartmentDetailsAsync(int id);
}