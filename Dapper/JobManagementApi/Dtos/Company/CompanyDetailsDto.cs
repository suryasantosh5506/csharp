using JobManagementApi.Dtos.Jobs;

namespace JobManagementApi.Dtos.Company;

public record CompanyDetailsDto(
    int Id,
    int UserId,
    string Name,
    string Description,
    string Location,
    string Website,
    IEnumerable<JobDto> Jobs
);