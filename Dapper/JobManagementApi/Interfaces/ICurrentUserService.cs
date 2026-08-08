using JobManagementApi.Enums;

namespace JobManagementApi.Interfaces;

public interface ICurrentUserService
{
    int UserId{get;}
    UserRole Role{get;}
    string Email { get; }
    bool IsAuthenticated { get; }
}