using HospitalManagementAPI.Dtos.DoctorApplication;
using HospitalManagementAPI.RequestHelpers;

namespace HospitalManagementAPI.Interfaces;

public interface IAdminDoctorApplicationService
{
    Task<PagedList<DoctorApplicationDetailsDto>> GetAllApplicationsAsync(PaginationParams paginationParams);

    Task<PagedList<DoctorApplicationDetailsDto>> GetAllPendingApplicationsAsync(PaginationParams paginationParams);

    Task<DoctorApplicationDetailsDto?> GetApplicationByIdAsync(int id);

    Task<DoctorApplicationDetailsDto?> RejectApplicationAsync(int id);

    Task<DoctorApplicationDetailsDto?> ApproveApplicationAsync(int id);
}