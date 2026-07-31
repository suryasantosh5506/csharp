using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.DoctorApplication;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.Extensions;
using HospitalManagementAPI.RequestHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Controllers;

[Authorize(Roles ="Admin")]
public class AdminDoctorApplicationController(HospitalContext context) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PagedList<DoctorApplicationDetailsDto>>> GetAllApplicationsAsync([FromQuery]PaginationParams paginationParams)
    {
        var query=context.DoctorApplications.Include(x=>x.User).Select(x=>x.ToDto());
        var applications=await PagedList<DoctorApplicationDetailsDto>.ToPagedList(query,paginationParams.pageNumber,paginationParams.pageSize);
        return Ok(applications);
    }

    [HttpGet("pending")]
    public async Task<ActionResult<PagedList<DoctorApplicationDetailsDto>>> GetAllPendingApplicationsAsync([FromQuery]PaginationParams paginationParams)
    {
        var query=context.DoctorApplications.Where(x=>x.Status=="pending").Include(x=>x.User).Select(x=>x.ToDto());
        var applications=await PagedList<DoctorApplicationDetailsDto>.ToPagedList(query,paginationParams.pageNumber,paginationParams.pageSize);
        return Ok(applications);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DoctorApplicationDetailsDto>> GetApplicationByIdAsync(int id)
    {
        var application = await context.DoctorApplications.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
        if (application is null) return NotFound();
        return Ok(application.ToDto());
    }

    [HttpPatch("{id:int}/reject")]
    public async Task<ActionResult<DoctorApplicationDetailsDto>> RejectApplicationAsync(int id)
    {
        List<string> validStatus = new() { "pending" };

        return await updateStatus(id, validStatus, "rejected");
    }

    [HttpPatch("{id:int}/approve")]
    public async Task<ActionResult<DoctorApplicationDetailsDto>> ApproveApplicationAsync(int id)
    {
        var application = await context.DoctorApplications.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);

        if (application is null) return NotFound();

        if (application.Status != "pending") return BadRequest("Application has already been processed.");

        var patient = await context.Patients.FirstOrDefaultAsync(x => x.UserId == application.UserId);

        if (patient is null) return NotFound("Patient profile not found.");

        var doctor = new Doctor
        {
            UserId = application.UserId,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            Email = patient.Email,
            PhoneNumber = patient.PhoneNumber,
            Qualification = application.Qualification,
            Specialization = application.Specialization,
            ExperienceYears = application.YearsOfExperience,
            LicenseNumber = application.LicenseNumber,
            HospitalName = application.HospitalName,
            Bio = application.Bio,
            DepartmentId = application.DepartmentId,
            ConsultationFee = application.ConsultationFee
        };

        context.Doctors.Add(doctor);
        application.User.Role = "Doctor";
        application.Status = "approved";
        await context.SaveChangesAsync();
        return Ok(application.ToDto());
    }

    private async Task<ActionResult<DoctorApplicationDetailsDto>> updateStatus(int applicationId,List<string>ValidStatus,string newStatus)
    {
        var application=await context.DoctorApplications.Include(x=>x.User).FirstOrDefaultAsync(x=>x.Id==applicationId);
        if(application is null) return NotFound();
        if (!ValidStatus.Any(x=>x==application.Status))
        {
            return BadRequest();
        }
        application.Status=newStatus;
        await context.SaveChangesAsync();
        return Ok(application.ToDto());
    }
}