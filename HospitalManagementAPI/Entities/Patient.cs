namespace HospitalManagementAPI.Entities;

public class Patient
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;


    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string Email { get; set; }

    public required string PhoneNumber { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public required string Gender { get; set; }

    public required string BloodGroup { get; set; }

    public int Height { get; set; }

    public decimal Weight { get; set; }

    public required string Address { get; set; }

    public required string EmergencyContactName { get; set; }

    public required string EmergencyContactPhone { get; set; }

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}