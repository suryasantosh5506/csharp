using HospitalManagementAPI.Dtos.Patient;
using HospitalManagementAPI.Entities;

namespace HospitalManagementAPI.Extensions;

public static class PatientExtension
{
    public static PatientDetailsDto ToDto(this Patient patient)
    {
        return new PatientDetailsDto(
            patient.Id,
            patient.FirstName,
            patient.LastName,
            patient.Email,
            patient.PhoneNumber,
            patient.DateOfBirth,
            patient.Gender,
            patient.BloodGroup,
            patient.Height,
            patient.Weight,
            patient.Address,
            patient.EmergencyContactName,
            patient.EmergencyContactPhone
        );
    }
}