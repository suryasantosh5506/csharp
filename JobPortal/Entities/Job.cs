namespace JobPortal.Entities;

public class Job
{
    public int Id{get;set;}
    public required string Title{get;set;}
    public required string Description{get;set;}
    public decimal Salary{get;set;}
    public required string Location{get;set;}
    public int Experience{get;set;}
    public int CompanyId{get;set;}
    public required Company Company{get;set;}
    public ICollection<Application> Applications { get; set; } = [];
}