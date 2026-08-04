using JobPortal.Enums;

namespace JobPortal.Entities;

public class Application
{
    public int Id{get;set;}
    public int UserId{get;set;}
    public required User User{get;set;}
    public int JobId{get;set;}
    public required Job Job{get;set;}
    public required string ResumeUrl{get;set;}
    public ApplicationStatus Status{get;set;}=ApplicationStatus.Pending;
    public DateTime AppliedAt{get;set;}
}