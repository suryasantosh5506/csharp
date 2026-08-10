using JobManagementApi.Dtos.Company;
using JobManagementApi.Dtos.JobApplication;
using JobManagementApi.Dtos.Jobs;
using JobManagementApi.Entities;

namespace JobManagementApi.Interfaces;

public interface IMappingService
{
    Task<JobDetailsDto> GetJobDetails(int jobId);
    Task<JobApplicationDetailsDto> GetApplicationDetails(int applicationId);
    Task<CompanyDetailsDto> GetCompanyDetails(int companyId);
    Task<JobWithSkillsDetailsDto> GetJobWithSkillsDetails(int jobId);
}