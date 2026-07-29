using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.Dtos.Department;

public record CreateDepartmentDto(
    [Required]
    [MaxLength(50)]
    string Name,
    [Required]
    string Description
);