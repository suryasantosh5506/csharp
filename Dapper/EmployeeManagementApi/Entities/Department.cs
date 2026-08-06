namespace EmployeeManagementApi.Entities;

public class Department
{
    public int Id {get;set;}
    public required string Name{get;set;}
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public List<Employee> Employees{get;set;}=[];
}