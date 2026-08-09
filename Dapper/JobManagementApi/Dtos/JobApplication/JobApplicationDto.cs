using JobManagementApi.Enums;

namespace JobManagementApi.Dtos.JobApplication;

public record JobApplicationDto(
    int Id,
    int JobId,
    int CandidateId,
    string ResumeUrl,
    ApplicationStatus Status
);