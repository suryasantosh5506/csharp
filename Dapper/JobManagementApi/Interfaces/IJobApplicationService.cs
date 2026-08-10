using JobManagementApi.Dtos.JobApplication;
using JobManagementApi.RequestHelpers.Pagination;

namespace JobManagementApi.Interfaces;

public interface IJobApplicationService
{
    Task<JobApplicationDto> CreateApplication(int jobId,CreateJobApplicationDto dto);

    Task<JobApplicationDto> GetApplicationById(int id);

    Task<PagedList<JobApplicationDto>> GetMyApplications(PaginationParams paginationParams);

    Task<PagedList<JobApplicationDto>> GetJobApplications(int jobId,PaginationParams paginationParams);

    Task<bool> UpdateApplicationStatus(int id,UpdateJobApplicationDto dto);

    Task<bool> DeleteApplication(int id);
}