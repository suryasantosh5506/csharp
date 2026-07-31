namespace HospitalManagementAPI.Dtos.DoctorApplication;

public record class CreateDoctorApplicationDto
(
    string Specialization,
    string Qualification,
    int YearsOfExperience,
    string HospitalName,
    int DepartmentId,
    decimal ConsultationFee,
    string Bio,
    string LicenseNumber
);