using HospitalManagementAPI.Dtos.Patient;

namespace HospitalManagementAPI.Interfaces;

public interface IPatientProfileService
{
    Task<PatientDetailsDto?> GetProfileAsync(int userId);

    Task<PatientDetailsDto> CreateProfileAsync(int userId,CreatePatientDto dto);

    Task<PatientDetailsDto?> UpdateProfileAsync(int userId,UpdatePatientDto dto);

    Task<bool> DeleteProfileAsync(int userId);
}