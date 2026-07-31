using HospitalManagementAPI.Entities;

namespace HospitalManagementAPI.Extensions;

public static class DoctorApplicationExtension
{
    public static DoctorApplicationDetailsDto ToDto(this DoctorApplication doctorApplication)
    {
        return new DoctorApplicationDetailsDto(
            doctorApplication.Id,
            doctorApplication.User.FirstName + " " + doctorApplication.User.LastName,
            doctorApplication.User.Email,
            doctorApplication.Specialization,
            doctorApplication.Qualification,
            doctorApplication.YearsOfExperience,
            doctorApplication.HospitalName,
            doctorApplication.Bio,
            doctorApplication.LicenseNumber,
            doctorApplication.Status,
            doctorApplication.AppliedAt
        );
    }
}