using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.DoctorApplication;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.Extensions;
using HospitalManagementAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Services;

public class DoctorApplicationService(HospitalContext context) : IDoctorApplicationService
{
    public async Task<DoctorApplicationDetailsDto?> GetMyApplicationAsync(int userId)
    {
        var application=await context.DoctorApplications
            .Include(x=>x.User)
            .FirstOrDefaultAsync(x=>x.UserId==userId);

        if(application is null) return null;

        return application.ToDto();
    }

    public async Task<DoctorApplicationDetailsDto> ApplyDoctorAsync(int userId,CreateDoctorApplicationDto applicationDto)
    {
        DoctorApplication application=new()
        {
            UserId=userId,
            Specialization=applicationDto.Specialization,
            Qualification=applicationDto.Qualification,
            YearsOfExperience=applicationDto.YearsOfExperience,
            HospitalName=applicationDto.HospitalName,
            Bio=applicationDto.Bio,
            LicenseNumber=applicationDto.LicenseNumber,
            Status="Pending",
            AppliedAt=DateTime.UtcNow
        };

        context.DoctorApplications.Add(application);

        await context.SaveChangesAsync();

        await context.Entry(application)
            .Reference(x=>x.User)
            .LoadAsync();

        return application.ToDto();
    }
}