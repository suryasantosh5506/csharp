using JobManagementApi.Dtos.Auth;
using JobManagementApi.Dtos.Company;
using JobManagementApi.Dtos.Jobs;
using JobManagementApi.Enums;

namespace JobManagementApi.Dtos.JobApplication;

public record JobApplicationDetailsDto(
    int Id,
    string ResumeUrl,
    ApplicationStatus Status,
    JobDetailsDto Job,
    UserSummaryDto Candidate
);