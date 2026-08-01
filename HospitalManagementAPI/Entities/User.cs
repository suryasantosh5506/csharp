using HospitalManagementAPI.enums;

namespace HospitalManagementAPI.Entities;

public class User
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
}