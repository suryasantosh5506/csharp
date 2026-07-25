using System.ComponentModel.DataAnnotations;
using StudentManagementAPI.Models;

namespace StudentManagementAPI.dtos.Students;

public record StudentDetailsDto(
    [Required]
    int Id,
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
    string Department,
    [Required]
    DateOnly EnrollmentDate
);