using LearnHubApi.Enums;

namespace LearnHubApi.Interfaces;

public interface ICurrentUserService
{
    int UserId { get; }

    string Email { get; }

    UserRole Role { get; }

    bool IsAuthenticated { get; }
}