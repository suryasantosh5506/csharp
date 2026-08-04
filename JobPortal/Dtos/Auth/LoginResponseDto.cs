using JobPortal.Enums;

namespace JobPortal.Dtos.Auth;

public record LoginResponseDto(
    int UserId,
    string token,
    string Email,
    string FullName,
    UserRole Role
);