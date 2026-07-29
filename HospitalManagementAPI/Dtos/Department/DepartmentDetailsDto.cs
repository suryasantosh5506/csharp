namespace HospitalManagementAPI.Dtos.Department;
using HospitalManagementAPI.Dtos.Doctor;

public record DepartmentDetailsDto
(
    int Id,
    string Name,
    string Description
);