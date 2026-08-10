using JobManagementApi.Dtos.Auth;
using JobManagementApi.Dtos.Company;
using JobManagementApi.Dtos.Skills;
using JobManagementApi.Enums;

namespace JobManagementApi.Dtos.Jobs;

public record JobWithSkillsDetailsDto(
    int Id,
    string Title,
    string Description,
    string Location,
    decimal SalaryMin,
    decimal SalaryMax,
    JobTypes JobType,
    int Experience,
    CompanyDto Company,
    UserSummaryDto Recruiter,
    IEnumerable<SkillDto> Skills
);