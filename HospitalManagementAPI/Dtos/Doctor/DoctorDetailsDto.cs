namespace HospitalManagementAPI.Dtos.Doctor;

public record DoctorDetailsDto(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Qualification,
    string Specialization,
    int ExperienceYears,
    decimal ConsultationFee,
    string LicenseNumber,
    int DepartmentId,
    string DepartmentName
);