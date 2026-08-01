namespace LearnHubApi.Entities;

public class Review
{
    public int Id {get;set;}
    public double Rating{get;set;}
    public required string Comment{get;set;}
    public int AuthorId{get;set;}
    public User Author{get;set;}=null!;
    public int CourseId{get;set;}
    public Course Course{get;set;}=null!;
    public DateTime CreatedAt{get;set;}
}