namespace JobPortal.Entities;

public class Company
{
    public int Id{get;set;}
    public required string Name{get;set;}
    public required string Description{get;set;}
    public required string Website{get;set;}
    public required string LogoUrl{get;set;}
    public int UserId{get;set;}
    public User User{get;set;}=null!;
    public ICollection<Job> Jobs{get;set;} =[];
}