using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.Dtos.DoctorAvailability;

public record UpdateDoctorAvailabilityDto(
    [Required]
    DayOfWeek DayOfWeek,

    [Required]
    TimeOnly StartTime,

    [Required]
    TimeOnly EndTime,

    bool IsAvailable
);