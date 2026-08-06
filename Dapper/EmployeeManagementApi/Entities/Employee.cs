namespace EmployeeManagementApi.Entities;

public class Employee
{
    public int Id{get;set;}
    public required string Name {get;set;}
    public required string Email {get;set;}
    public required string Phone {get;set;}
    public Address? Address { get; set; }
    public Company Company{get;set;}=null!;
    public required int DepartmentId {get;set;}
    public Department Department{get;set;}=null!;
}