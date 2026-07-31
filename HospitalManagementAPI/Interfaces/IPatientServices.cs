using HospitalManagementAPI.Dtos.Patient;
using HospitalManagementAPI.RequestHelpers;

namespace HospitalManagementAPI.Interfaces;
public interface IPatientService
{
    Task<PagedList<PatientDetailsDto>> GetAllPatientsAsync(PaginationParams paginationParams);
    Task<PatientDetailsDto?> GetPatientByIdAsync(int id);
    Task<PatientDetailsDto> CreatePatientAsync(CreatePatientDto dto);
    Task<PatientDetailsDto?> UpdatePatientAsync(int id, UpdatePatientDto dto);
    Task<bool> DeletePatientAsync(int id);
}