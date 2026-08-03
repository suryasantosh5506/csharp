namespace LearnHubApi.Entities;

public class Course
{
    public int Id {get;set;}
    public required string Title {get;set;}
    public required string Description {get;set;}
    public required string Thumbnail {get;set;}
    public required string PublicId {get;set;}
    public required decimal Price {get;set;}
    public required string Language {get;set;}
    public required double Duration {get;set;}
    public DateTime CreatedAt{get;set;}
    public DateTime UpdatedAt{get;set;}
    public int InstructorId {get;set;}
    public User Instructor {get;set;}=null!;
    public int CategoryId {get;set;}
    public Category Category {get;set;}=null!;
    public ICollection<Module> Modules{ get; set; }=new List<Module>();
    public ICollection<Review> Reviews{ get; set; }=new List<Review>();
    public ICollection<Enrollment> Enrollments{ get; set; }=new List<Enrollment>();
}