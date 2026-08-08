namespace JobManagementApi.Entities;

public class Company
{
    public int Id{get;set;}
    public required int UserId{get;set;}
    public required string Name{get;set;}
    public string Description{get;set;}=string.Empty;
    public required string Location{get;set;}
    public required string Website{get;set;}
    public User User{get;set;}=null!;
    public List<Job> Jobs{get;set;}=[];
}