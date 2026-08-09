using System.Security.AccessControl;
using JobManagementApi.Dtos.Company;
using JobManagementApi.Dtos.Jobs;
using JobManagementApi.Entities;

namespace JobManagementApi.Extensions;

public static class JobExtension
{
    public static JobDto ToDto(this Job job)
    {
        return new(job.Id,job.CompanyId,job.RecruiterId,job.Title,job.Description,job.Location,job.SalaryMin,job.SalaryMax,job.JobType,
                job.Experience);
    }
}