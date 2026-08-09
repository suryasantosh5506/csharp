using JobManagementApi.Dtos.JobApplication;
using JobManagementApi.Entities;

namespace JobManagementApi.Extensions;

public static class JobApplicationExtension
{
    public static JobApplicationDto ToDto(this Application application)
    {
        return new(application.Id,application.JobId,application.CandidateId,application.ResumeUrl,application.Status);
    }
}