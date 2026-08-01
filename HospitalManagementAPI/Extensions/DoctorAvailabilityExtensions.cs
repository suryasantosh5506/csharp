using HospitalManagementAPI.Dtos.DoctorAvailability;
using HospitalManagementAPI.Entities;

namespace HospitalManagementAPI.Extensions;

public static class DoctorAvailabilityExtensions
{
    public static DoctorAvailabilityDetailsDto ToDto(this DoctorAvailability availability)
    {
        return new DoctorAvailabilityDetailsDto(
            availability.Id,
            availability.DayOfWeek,
            availability.StartTime,
            availability.EndTime,
            availability.IsAvailable
        );
    }
}