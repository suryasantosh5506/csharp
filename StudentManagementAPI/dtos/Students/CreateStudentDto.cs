using System.ComponentModel.DataAnnotations;

namespace StudentManagementAPI.dtos.Students;

public record CreateStudentDto(
    [Required]
    [StringLength(50)]
    string FirstName,
    [Required]
    [StringLength(50)]
    string LastName,
    [Required]
    [StringLength(25)]
    string Email,
    [Required]
    int Age,
    [Required]
    int DepartmentId,
    [Required]
    DateOnly EnrollmentDate
);