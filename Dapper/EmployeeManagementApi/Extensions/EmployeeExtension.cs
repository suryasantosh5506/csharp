using EmployeeManagementApi.Dtos.Employee;
using EmployeeManagementApi.Entities;

namespace EmployeeManagementApi.Extensions;

public static class EmployeeExtension{
    public static EmployeeDto ToDto(this Employee employee)
    {
        return new(employee.Id,employee.Name,employee.Email,employee.Phone,employee.CompanyId,employee.DepartmentId);
    }
}