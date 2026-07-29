namespace HospitalManagementAPI.Dtos.Patient;

public record PatientDetailsDto(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateOnly DateOfBirth,
    string Gender,
    string BloodGroup,
    int Height,
    decimal Weight,
    string Address,
    string EmergencyContactName,
    string EmergencyContactPhone
);