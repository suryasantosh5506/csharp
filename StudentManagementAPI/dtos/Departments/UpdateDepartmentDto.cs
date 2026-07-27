using System.ComponentModel.DataAnnotations;

namespace StudentManagementAPI.dtos.Departments;

public record UpdateDepartmentDto(
    [Required]
    [StringLength(25)]
    string Name
);