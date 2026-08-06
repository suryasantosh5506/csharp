namespace EmployeeManagementApi.Entities;

public class Address
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string HouseNo { get; set; } = string.Empty;

    public string Street { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public Employee Employee { get; set; }=null!;
}