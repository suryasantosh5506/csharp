using JobManagementApi.Enums;

namespace JobManagementApi.Dtos.Auth;

public record UserSummaryDto(
    int Id,
    string Name,
    string Email,
    UserRole Role
);