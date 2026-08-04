using JobPortal.Enums;

namespace JobPortal.Entities;


public class User
{
    public int Id{get;set;}
    public required string FullName{get;set;}
    public required string Email{get;set;}
    public required string PasswordHash{get;set;}
    public UserRole Role{get;set;}=UserRole.JobSeeker;
    public string? ProfileImageUrl{get;set;}
    public DateTime CreatedAt{get;set;}

    public ICollection<Company> Companies { get; set; } = [];
    public ICollection<Application> Applications { get; set; } = [];
}