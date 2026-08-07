using System.ComponentModel.DataAnnotations;
using EmployeeManagementApi.Entities;

namespace EmployeeManagementApi.Dtos.Employee;

public record EmployeeDetailsDto(
    int Id,
    string Name,
    string Email,
    string Phone,
    int CompanyId,
    int DepartmentId,
    Address Address
);