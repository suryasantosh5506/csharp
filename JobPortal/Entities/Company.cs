namespace JobPortal.Entities;

public class Company
{
    public int Id{get;set;}
    public required string Name{get;set;}
    public required string Description{get;set;}
    public required string Website{get;set;}
    public required string LogoUrl{get;set;}
    public int UserId{get;set;}
    public required User User{get;set;}
    public ICollection<Job> Jobs{get;set;} =[];
}