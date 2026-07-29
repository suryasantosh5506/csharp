namespace HospitalManagementAPI.Dtos.Appointment;

public record AppointmentDetailsDto(
    int Id,
    int DoctorId,
    string DoctorName,
    int PatientId,
    string PatientName,
    DateOnly AppointmentDate,
    TimeOnly AppointmentTime,
    string Reason,
    string Status
);