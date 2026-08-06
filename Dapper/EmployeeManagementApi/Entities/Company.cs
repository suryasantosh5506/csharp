namespace EmployeeManagementApi.Entities;
public class Company
{
    public int Id{get;set;}
    public required string Name{get;set;}
    public required string Email{get;set;}
    public required string Phone{get;set;}
    public List<Department> Departments { get; set; } = [];
    public List<Employee> Employees { get; set; } = [];

}