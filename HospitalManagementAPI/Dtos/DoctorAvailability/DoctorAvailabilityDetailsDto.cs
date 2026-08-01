namespace HospitalManagementAPI.Dtos.DoctorAvailability;

public record DoctorAvailabilityDetailsDto(
    int Id,
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    bool IsAvailable
);