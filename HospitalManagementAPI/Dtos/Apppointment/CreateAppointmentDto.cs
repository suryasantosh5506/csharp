using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.Dtos.Appointment;

public record CreateAppointmentDto(
    [Range(1, int.MaxValue)]
    int DoctorId,

    [Required]
    DateOnly AppointmentDate,

    [Required]
    TimeOnly AppointmentTime,

    [Required]
    [MaxLength(500)]
    string Reason
);