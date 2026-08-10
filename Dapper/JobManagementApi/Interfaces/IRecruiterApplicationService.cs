using JobManagementApi.Dtos.RecruiterApplication;
using JobManagementApi.RequestHelpers.Pagination;

namespace JobManagementApi.Interfaces;

public interface IRecruiterApplicationService
{
    Task<RecruiterApplicationDto> CreateApplication(CreateRecruiterApplicationDto dto);

    Task<PagedList<RecruiterApplicationDto>> GetMyApplications(PaginationParams paginationParams);

    Task<PagedList<RecruiterApplicationDto>> GetApplications(PaginationParams paginationParams);

    Task<RecruiterApplicationDto> GetApplicationById(int id);

    Task<bool> UpdateApplication(int id,UpdateRecruiterApplicationDto dto);

    Task<bool> DeleteApplication(int id);
}