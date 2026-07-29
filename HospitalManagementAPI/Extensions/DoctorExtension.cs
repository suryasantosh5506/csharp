using HospitalManagementAPI.Dtos.Doctor;
using HospitalManagementAPI.Entities;

namespace HospitalManagementAPI.Extensions;

public static class DoctorExtension
{
    public static DoctorDetailsDto ToDto(this Doctor doctor)
    {
        return new DoctorDetailsDto(
            doctor.Id,
            doctor.FirstName,
            doctor.LastName,
            doctor.Email,
            doctor.PhoneNumber,
            doctor.Qualification,
            doctor.Specialization,
            doctor.ExperienceYears,
            doctor.ConsultationFee,
            doctor.LicenseNumber,
            doctor.DepartmentId,
            doctor.Department.Name
        );
    }
}