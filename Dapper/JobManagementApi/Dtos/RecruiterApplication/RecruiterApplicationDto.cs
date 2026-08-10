using JobManagementApi.Enums;

namespace JobManagementApi.Dtos.RecruiterApplication;

public record RecruiterApplicationDto(
    int Id,
    int CandidateId,
    string Reason,
    RecruiterApplicationStatus Status,
    DateTime AppliedAt,
    DateTime? ReviewedAt,
    int? ReviewedBy
);