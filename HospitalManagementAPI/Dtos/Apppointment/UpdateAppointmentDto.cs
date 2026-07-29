using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.Dtos.Appointment;

public record UpdateAppointmentDto(
    [Required]
    DateOnly AppointmentDate,

    [Required]
    TimeOnly AppointmentTime,

    [Required]
    [MaxLength(500)]
    string Reason,

    [Required]
    string Status
);