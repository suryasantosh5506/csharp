namespace LearnHubApi.Entities;

public class Module
{
    public int Id {get;set;}
    public required string Title {get;set;}
    public string Description{get;set;}=string.Empty;
    public int Order{get;set;}
    public int CourseId{get;set;}
    public Course Course{get;set;}=null!;
    public DateTime CreatedAt{get;set;}
    public ICollection<Lesson>Lessons{ get; set; }=new List<Lesson>();
}