using HospitalManagementAPI.Dtos.DoctorAvailability;
using HospitalManagementAPI.RequestHelpers;

namespace HospitalManagementAPI.Interfaces;

public interface IDoctorAvailabilityService
{
    Task<PagedList<DoctorAvailabilityDetailsDto>> GetDoctorAvailabilityAsync(int doctorId,PaginationParams paginationParams);

    Task<DoctorAvailabilityDetailsDto?> GetAvailabilityByIdAsync(int id,int doctorId);

    Task<DoctorAvailabilityDetailsDto> CreateAvailabilityAsync(int doctorId,CreateDoctorAvailabilityDto dto);

    Task<DoctorAvailabilityDetailsDto?> UpdateAvailabilityAsync(int id,int doctorId,UpdateDoctorAvailabilityDto dto);

    Task<bool> DeleteAvailabilityAsync(int id,int doctorId);
}