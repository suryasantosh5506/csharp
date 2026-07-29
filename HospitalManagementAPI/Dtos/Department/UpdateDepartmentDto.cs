using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.Dtos.Department;

public record UpdateDepartmentDto(
    [Required]
    [MaxLength(50)]
    string Name,
    [Required]
    string Description
);