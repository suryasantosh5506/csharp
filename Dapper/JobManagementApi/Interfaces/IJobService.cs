using JobManagementApi.Dtos.Jobs;
using JobManagementApi.RequestHelpers.Pagination;

namespace JobManagementApi.Interfaces;

public interface IJobService
{
    Task<JobDto> CreateJob(CreateJobDto dto);

    Task<PagedList<JobDto>> GetJobs(PaginationParams paginationParams);

    Task<JobDto> GetJobById(int id);

    Task<bool> UpdateJob(int id, UpdateJobDto dto);

    Task<bool> DeleteJob(int id);
}