using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.Dtos.Patient;

public record CreatePatientDto(
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
    [StringLength(15, MinimumLength = 10)]
    string PhoneNumber,

    [Required]
    DateOnly DateOfBirth,

    [Required]
    [MaxLength(10)]
    string Gender,

    [Required]
    [MaxLength(5)]
    string BloodGroup,

    [Range(30, 300)]
    int Height,

    [Range(1, 500)]
    decimal Weight,

    [Required]
    [MaxLength(500)]
    string Address,

    [Required]
    [MaxLength(100)]
    string EmergencyContactName,

    [Required]
    [Phone]
    [StringLength(15, MinimumLength = 10)]
    string EmergencyContactPhone
);