namespace JobManagementApi.Entities;

public class JobSkills
{
    public required int JobId{get;set;}
    public required int SkillId{get;set;}
    public Job Job{get;set;}=null!;
    public Skills Skill{get;set;}=null!;
}