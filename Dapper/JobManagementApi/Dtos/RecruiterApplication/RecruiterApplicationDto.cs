using JobManagementApi.Enums;

namespace JobManagementApi.Dtos.RecruiterApplication;

public record RecruiterApplication(
    int Id,
    int CandidateId,
    string Reason,
    RecruiterApplicationStatus Status,
    DateTime AppliedAt,
    DateTime? ReviewedAt,
    int? ReviewedBy
);