namespace LearnHubApi.Entities;

public class Category
{
    public int Id {get;set;}
    public required string Name {get;set;}
    public string Description {get;set;}=string.Empty;
    public ICollection<Course> Courses{ get; set; }=new List<Course>();
}