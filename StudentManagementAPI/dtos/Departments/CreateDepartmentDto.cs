using System.ComponentModel.DataAnnotations;

namespace StudentManagementAPI.dtos.Departments;

public record CreateDepartmentDto
(
    [Required]
    [StringLength(20)]
    string Name
);