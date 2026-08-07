using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementApi.Dtos.Employee;

public record EmployeeDto(
    int Id,
    string Name,
    string Email,
    string Phone,
    int CompanyId,
    int DepartmentId
);