using HospitalManagementAPI.Dtos.Doctor;
using HospitalManagementAPI.RequestHelpers;

namespace HospitalManagementAPI.Interfaces;

public interface IDoctorService
{
    Task<PagedList<DoctorDetailsDto>> GetAllDoctorsAsync(DoctorParams doctorParams);

    Task<DoctorDetailsDto?> GetDoctorByIdAsync(int id);

    Task<DoctorDetailsDto> CreateDoctorAsync(CreateDoctorDto dto);

    Task<DoctorDetailsDto?> UpdateDoctorAsync(int id, UpdateDoctorDto dto);

    Task<bool> DeleteDoctorAsync(int id);
}