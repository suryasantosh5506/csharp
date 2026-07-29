using HospitalManagementAPI.Dtos.Department;
using HospitalManagementAPI.Entities;

namespace HospitalManagementAPI.Extensions;

public static class DepartmentExtensions
{
    public static DepartmentDetailsDto ToDto(this Department department)
    {
        return new DepartmentDetailsDto(
            department.Id,
            department.Name,
            department.Description
        );
    }
}