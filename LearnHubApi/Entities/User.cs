using LearnHubApi.Enums;

namespace LearnHubApi.Entities;

public class User
{
    public int Id {get;set;}
    public required string FirstName{get;set;}
    public required string LastName{get;set;}
    public required string Email{get;set;}
    public required string PasswordHash{get;set;}
    public UserRole Role{get;set;}=UserRole.Student;
    public string Bio{get;set;}=string.Empty;
    public DateTime CreatedAt{get;set;}
    public DateTime UpdatedAt{get;set;}
    public string ProfilePic{get;set;}=string.Empty;
    public ICollection<Course> Courses{ get; set; }=new List<Course>();
    public ICollection<Enrollment> Enrollments{ get; set; }=new List<Enrollment>();
    public ICollection<Review> Reviews{ get; set; }=new List<Review>();
}