using JobManagementApi.Enums;

namespace JobManagementApi.Entities;

public class RecruiterApplication
{
    public int Id{get;set;}
    public int CandidateId{get;set;}
    public User Candidate{get;set;}=null!;
    public required string Reason{get;set;}
    public required RecruiterApplicationStatus Status{get;set;}
    public required DateTime AppliedAt{get;set;}
    public DateTime ReviewedAt{get;set;}
    public int ReviewedBy{get;set;}
    public User? Reviewer{get;set;}
}