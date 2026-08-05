using JobPortal.Enums;

namespace JobPortal.Interfaces;

public interface ICurrentUserService
{
    int UserID{get;}
    UserRole Role{get;}
    string Email { get; }
    bool IsAuthenticated{get;}
}