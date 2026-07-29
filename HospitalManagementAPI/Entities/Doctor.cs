namespace HospitalManagementAPI.Entities;

public class Doctor
{
    public int Id { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string Email { get; set; }

    public required string PhoneNumber { get; set; }

    public required string Qualification { get; set; }

    public required string Specialization { get; set; }

    public int ExperienceYears { get; set; }

    public decimal ConsultationFee { get; set; }

    public required string LicenseNumber { get; set; }

    public int DepartmentId { get; set; }

    public Department Department { get; set; } = null!;

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}