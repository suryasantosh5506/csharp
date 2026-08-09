using JobManagementApi.Dtos.Jobs;

namespace JobManagementApi.Interfaces;

public interface IJobService
{
    Task<JobDto> CreateJob(CreateJobDto dto);

    Task<IEnumerable<JobDto>> GetJobs();

    Task<JobDto> GetJobById(int id);

    Task<bool> UpdateJob(int id, UpdateJobDto dto);

    Task<bool> DeleteJob(int id);
}