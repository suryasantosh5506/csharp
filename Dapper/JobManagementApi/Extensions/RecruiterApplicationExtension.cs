using JobManagementApi.Dtos.RecruiterApplication;
using JobManagementApi.Entities;

namespace JobManagementApi.Extensions;

public static class RecruiterApplicationExtension
{
    public static RecruiterApplicationDto ToDto(this RecruiterApplication application)
    {
        return new(application.Id,application.CandidateId,application.Reason,application.Status,application.AppliedAt,application.ReviewedAt,application.ReviewedBy);
    }
}