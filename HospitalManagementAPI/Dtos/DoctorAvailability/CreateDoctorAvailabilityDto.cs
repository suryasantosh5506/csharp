using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.Dtos.DoctorAvailability;

public record CreateDoctorAvailabilityDto(
    [Required]
    DayOfWeek DayOfWeek,

    [Required]
    TimeOnly StartTime,

    [Required]
    TimeOnly EndTime
);