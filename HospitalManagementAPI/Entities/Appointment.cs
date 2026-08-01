using HospitalManagementAPI.enums;

namespace HospitalManagementAPI.Entities;

public class Appointment
{
    public int Id { get; set; }

    public int DoctorId { get; set; }

    public Doctor Doctor { get; set; } = null!;

    public int PatientId { get; set; }

    public Patient Patient { get; set; } = null!;

    public DateOnly AppointmentDate { get; set; }

    public TimeOnly AppointmentTime { get; set; }

    public required string Reason { get; set; }

    public required AppointmentStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
}