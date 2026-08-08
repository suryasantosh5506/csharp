using JobManagementApi.Enums;

namespace JobManagementApi.Entities;

public class Application
{
    public int Id{get;set;}
    public required int JobId{get;set;}
    public required int CandidateId{get;set;}
    public required string ResumeUrl{get;set;}
    public required ApplicationStatus Status{get;set;}
    public Job Job{get;set;}=null!;
    public User Candidate{get;set;}=null!;
}