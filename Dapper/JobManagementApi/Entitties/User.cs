using JobManagementApi.Enums;

namespace JobManagementApi.Entities;

public class User
{
    public int Id{get;set;}
    public required string Name{get;set;}
    public required string Email{get;set;}
    public required string PasswordHash{get;set;}
    public UserRole Role{get;set;}=UserRole.Candidate;
    public List<Application> Applications{get;set;}=[];
    public List<Company> Companies { get; set; } = [];
}