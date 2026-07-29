namespace HospitalManagementAPI.Entities;

public class Department
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}