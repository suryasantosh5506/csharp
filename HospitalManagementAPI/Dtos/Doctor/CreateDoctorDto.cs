using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.Dtos.Doctor;

public record CreateDoctorDto(
    [Required]
    [MaxLength(50)]
    string FirstName,

    [Required]
    [MaxLength(50)]
    string LastName,

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    string Email,

    [Required]
    [Phone]
    [MaxLength(15)]
    string PhoneNumber,

    [Required]
    [MaxLength(100)]
    string Qualification,

    [Required]
    [MaxLength(100)]
    string Specialization,

    [Range(0, 60)]
    int ExperienceYears,

    [Range(0.0, 100000.0)]
    decimal ConsultationFee,

    [Required]
    [MaxLength(50)]
    string LicenseNumber,

    [Range(1, int.MaxValue)]
    int DepartmentId
);