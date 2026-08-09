using JobManagementApi.Dtos.JobApplication;

namespace JobManagementApi.Interfaces;

public interface IJobApplicationService
{
    Task<JobApplicationDto> CreateApplication(int jobId,CreateJobApplicationDto dto);

    Task<JobApplicationDto> GetApplicationById(int id);

    Task<IEnumerable<JobApplicationDto>> GetMyApplications();

    Task<IEnumerable<JobApplicationDto>> GetJobApplications(int jobId);

    Task<bool> UpdateApplicationStatus(int id,UpdateJobApplicationDto dto);

    Task<bool> DeleteApplication(int id);
}