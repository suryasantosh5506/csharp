using System.Security.Claims;
using LearnHubApi.Enums;
using LearnHubApi.Interfaces;

namespace LearnHubApi.Services;

public class CurrentUserService(IHttpContextAccessor contextAccessor) : ICurrentUserService
{
    public int UserId => int.Parse(contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public string Email => contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email)!;

    public UserRole Role => Enum.Parse<UserRole>(contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role)!);

    public bool IsAuthenticated =>
    contextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}