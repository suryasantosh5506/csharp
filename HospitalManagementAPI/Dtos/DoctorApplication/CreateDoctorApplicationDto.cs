public record class CreateDoctorApplicationDto
(
    string Specialization,
    string Qualification,
    int YearsOfExperience,
    string HospitalName,
    string Bio,
    string LicenseNumber
);