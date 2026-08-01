using System.ComponentModel.DataAnnotations;
using HospitalManagementAPI.enums;

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
    AppointmentStatus Status
);