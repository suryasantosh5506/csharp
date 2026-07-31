namespace HospitalManagementAPI.Entities;

public class DoctorApplication
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string Specialization { get; set; } = string.Empty;

    public string Qualification { get; set; } = string.Empty;

    public int YearsOfExperience { get; set; }

    public string HospitalName { get; set; } = string.Empty;

    public int DepartmentId { get; set; }

    public decimal ConsultationFee { get; set; }

    public string Bio { get; set; } = string.Empty;

    public string LicenseNumber { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";

    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
}