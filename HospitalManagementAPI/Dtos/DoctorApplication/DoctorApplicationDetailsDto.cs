namespace HospitalManagementAPI.Dtos.DoctorApplication;

public record class DoctorApplicationDetailsDto
(
    int Id,
    string ApplicantName,
    string ApplicantEmail,
    string Specialization,
    string Qualification,
    int YearsOfExperience,
    string HospitalName,
    string Bio,
    string LicenseNumber,
    string Status,
    DateTime AppliedAt
);