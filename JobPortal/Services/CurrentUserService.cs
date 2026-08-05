using System.Security.Claims;
using JobPortal.Enums;
using JobPortal.Interfaces;

namespace JobPortal.Services;

public class CurrentUserService(IHttpContextAccessor context) : ICurrentUserService
{
    public int UserID => int.Parse(context.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public UserRole Role => Enum.Parse<UserRole>(context.HttpContext?.User?.FindFirstValue(ClaimTypes.Role)!);

    public string Email => context.HttpContext?.User?.FindFirstValue(ClaimTypes.Email)!;

    public bool IsAuthenticated => context.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}