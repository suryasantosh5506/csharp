using JobManagementApi.Enums;

namespace JobManagementApi.Entities;

public class Job
{
    public int Id{get;set;}
    public required int CompanyId{get;set;}
    public required int RecruiterId{get;set;}
    public required string Title{get;set;}
    public string Description{get;set;}=string.Empty;
    public required string Location{get;set;}
    public decimal SalaryMin{get;set;}
    public decimal SalaryMax{get;set;}
    public JobTypes JobType{get;set;}
    public int Experience{get;set;}=0;
    public User Recruiter{get;set;}=null!;
    public Company Company { get; set; } = null!;
    public List<JobSkills> JobSkills {get;set;}=[];
}