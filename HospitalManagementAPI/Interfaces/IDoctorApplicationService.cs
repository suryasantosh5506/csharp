using HospitalManagementAPI.Dtos.DoctorApplication;

namespace HospitalManagementAPI.Interfaces;

public interface IDoctorApplicationService
{
    Task<DoctorApplicationDetailsDto?> GetMyApplicationAsync(int userId);

    Task<DoctorApplicationDetailsDto> ApplyDoctorAsync(int userId,CreateDoctorApplicationDto applicationDto);
}