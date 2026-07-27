using System.ComponentModel.DataAnnotations;

namespace StudentManagementAPI.dtos.Departments;

public record DepartmentDetailsDto(
    [Required]
    [StringLength(25)]
    string Name
);