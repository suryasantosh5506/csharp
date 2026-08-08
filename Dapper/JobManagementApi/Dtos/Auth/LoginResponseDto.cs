using System.ComponentModel.DataAnnotations;
using JobManagementApi.Enums;

namespace JobManagementApi.Dtos.Auth;

public record LoginResponseDto(
    int Id,
    string Name,
    string Email,
    UserRole Role,
    string Token
);