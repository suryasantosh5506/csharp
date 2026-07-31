using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.DoctorApplication;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.Extensions;
using HospitalManagementAPI.Interfaces;
using HospitalManagementAPI.RequestHelpers;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Services;

public class AdminDoctorApplicationService(HospitalContext context) : IAdminDoctorApplicationService
{
    public async Task<PagedList<DoctorApplicationDetailsDto>> GetAllApplicationsAsync(PaginationParams paginationParams)
    {
        var query=context.DoctorApplications
            .Include(x=>x.User)
            .Select(x=>x.ToDto());

        return await PagedList<DoctorApplicationDetailsDto>.ToPagedList(
            query,
            paginationParams.pageNumber,
            paginationParams.pageSize);
    }

    public async Task<PagedList<DoctorApplicationDetailsDto>> GetAllPendingApplicationsAsync(PaginationParams paginationParams)
    {
        var query=context.DoctorApplications
            .Where(x=>x.Status=="Pending")
            .Include(x=>x.User)
            .Select(x=>x.ToDto());

        return await PagedList<DoctorApplicationDetailsDto>.ToPagedList(
            query,
            paginationParams.pageNumber,
            paginationParams.pageSize);
    }

    public async Task<DoctorApplicationDetailsDto?> GetApplicationByIdAsync(int id)
    {
        var application=await context.DoctorApplications
            .Include(x=>x.User)
            .FirstOrDefaultAsync(x=>x.Id==id);

        if(application is null) return null;

        return application.ToDto();
    }

    public async Task<DoctorApplicationDetailsDto?> RejectApplicationAsync(int id)
    {
        var application=await context.DoctorApplications
            .Include(x=>x.User)
            .FirstOrDefaultAsync(x=>x.Id==id);

        if(application is null) return null;

        application.Status="Rejected";

        await context.SaveChangesAsync();

        return application.ToDto();
    }

    public async Task<DoctorApplicationDetailsDto?> ApproveApplicationAsync(int id)
    {
        var application=await context.DoctorApplications
            .Include(x=>x.User)
            .FirstOrDefaultAsync(x=>x.Id==id);

        if(application is null)
            return null;

        if(application.Status!="Pending")
            return null;

        var patient=await context.Patients
            .FirstOrDefaultAsync(x=>x.UserId==application.UserId);

        if(patient is null)
            return null;

        Doctor doctor=new()
        {
            UserId=application.UserId,
            FirstName=patient.FirstName,
            LastName=patient.LastName,
            Email=patient.Email,
            PhoneNumber=patient.PhoneNumber,
            Qualification=application.Qualification,
            Specialization=application.Specialization,
            ExperienceYears=application.YearsOfExperience,
            LicenseNumber=application.LicenseNumber,
            HospitalName=application.HospitalName,
            Bio=application.Bio,
            DepartmentId=application.DepartmentId,
            ConsultationFee=application.ConsultationFee
        };

        context.Doctors.Add(doctor);

        application.User.Role="Doctor";
        application.Status="Approved";

        await context.SaveChangesAsync();

        return application.ToDto();
    }
}