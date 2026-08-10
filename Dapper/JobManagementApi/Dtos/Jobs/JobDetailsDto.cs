using JobManagementApi.Dtos.Company;
using JobManagementApi.Dtos.Auth;
using JobManagementApi.Enums;

namespace JobManagementApi.Dtos.Jobs;

public record JobDetailsDto(
    int Id,
    string Title,
    string Description,
    string Location,
    decimal SalaryMin,
    decimal SalaryMax,
    JobTypes JobType,
    int Experience,
    CompanyDto Company,
    UserSummaryDto Recruiter
);