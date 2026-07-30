using HospitalManagementAPI.Dtos.Appointment;
using HospitalManagementAPI.Entities;

namespace HospitalManagementAPI.Extensions;

public static class AppointmentExtensions
{
    public static AppointmentDetailsDto ToDto(this Appointment appointment)
    {
        return new AppointmentDetailsDto(
            appointment.Id,
            appointment.DoctorId,
            appointment.Doctor.FirstName+appointment.Doctor.LastName,
            appointment.PatientId,
            appointment.Patient.FirstName+appointment.Patient.LastName,
            appointment.AppointmentDate,
            appointment.AppointmentTime,
            appointment.Reason,
            appointment.Status
        );
    }
}