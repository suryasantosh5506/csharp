using JobManagementApi.Enums;

namespace JobManagementApi.Dtos.Jobs;

public record JobDto(
    int Id,
    int CompanyId,
    int RecruiterId,
    string Title,
    string Description,
    string Location,
    decimal SalaryMin,
    decimal SalaryMax,
    JobTypes JobType,
    int Experience
);