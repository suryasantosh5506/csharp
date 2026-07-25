namespace StudentManagementAPI.Models;

public class Student
{
    public int Id{get;set;}
    public required string FirstName{get;set;}
    public required string LastName{get;set;}
    public required string Email{get;set;}
    public int Age{get;set;}
    public int DepartmentId{get;set;}
    public Department Department{get;set;}
    public DateOnly EnrollmentDate{get;set;}
}