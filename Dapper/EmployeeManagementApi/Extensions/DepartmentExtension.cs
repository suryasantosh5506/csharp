using EmployeeManagementApi.Dtos.Department;
using EmployeeManagementApi.Entities;

namespace EmployeeManagementApi.Extensions;

public static class DepartmentExtension{
    public static DepartmentDto ToDto(this Department department)
    {
        return new(department.Id,
            department.Name,
            department.CompanyId);
    }
}