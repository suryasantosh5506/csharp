namespace LearnHubApi.Entities;

public class Lesson
{
    public int Id{get;set;}
    public required string Title{get;set;}
    public string Description{get;set;}=string.Empty;
    public required string VideoUrl{get;set;}
    public required string PublicId{get;set;}
    public double Duration{get;set;}
    public int Order{get;set;}
    public int ModuleId{get;set;}
    public Module Module{get;set;}=null!;
    public DateTime CreatedAt{get;set;}
}