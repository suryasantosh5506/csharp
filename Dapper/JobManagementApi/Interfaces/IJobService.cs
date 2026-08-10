using JobManagementApi.Dtos.Jobs;
using JobManagementApi.RequestHelpers.Pagination;
using JobManagementApi.RequestHelpers.Searching;

namespace JobManagementApi.Interfaces;

public interface IJobService
{
    Task<JobDto> CreateJob(CreateJobDto dto);

    Task<PagedList<JobDto>> GetJobs(JobParams jobParams);

    Task<JobDto> GetJobById(int id);

    Task<bool> UpdateJob(int id, UpdateJobDto dto);

    Task<bool> DeleteJob(int id);
}