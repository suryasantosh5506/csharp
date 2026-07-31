using System.Runtime.InteropServices;
using System.Security.Claims;
using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Appointment;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Controllers;

[Authorize(Roles = "Doctor")]
public class DoctorAppointmentController(HospitalContext context) : BaseApiController
{
    private async Task<Doctor?> GetCurrentDoctorAsync()
    {
        var userId=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await context.Doctors.FirstOrDefaultAsync(x=>x.UserId==userId);
    }

    [HttpGet]
    public async Task<ActionResult<List<AppointmentDetailsDto>>> GetAllAppointmentsAsync()
    {
        var doctor=await GetCurrentDoctorAsync();
        if(doctor is null) return Unauthorized();
        var appointments=await context.Appointments.Include(x=>x.Doctor).Include(x=>x.Patient).Where(x=>x.DoctorId==doctor.Id).Select(x=>x.ToDto()).ToListAsync();
        return Ok(appointments);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppointmentDetailsDto>> GetAppointmentByIdAsync(int id)
    {
        var doctor=await GetCurrentDoctorAsync();
        if(doctor is null) return Unauthorized();
        var appointment=await context.Appointments.Include(x=>x.Patient).Include(x=>x.Doctor).FirstOrDefaultAsync(x=>x.Id==id && x.DoctorId==doctor.Id);
        if(appointment is null) return NotFound();
        return Ok(appointment.ToDto());
    }

    [HttpPatch("{id:int}/approve")]
    public async Task<ActionResult<AppointmentDetailsDto>> ApproveAppointmentAsync(int id)
    {
        List<string>ValidStatus=new(){"pending"};
        return await UpdateAppointmentStatusAsync(id,ValidStatus,"approve");
    }

    [HttpPatch("{id:int}/reject")]
    public async Task<ActionResult<AppointmentDetailsDto>> RejectAppointmentAsync(int id)
    {
        List<string>ValidStatus=new(){"pending"};
        return await UpdateAppointmentStatusAsync(id,ValidStatus,"reject");
    }

    [HttpPatch("{id:int}/complete")]
    public async Task<ActionResult<AppointmentDetailsDto>> CompleteAppointmentAsync(int id)
    {
        List<string>ValidStatus=new(){"approved"};
        return await UpdateAppointmentStatusAsync(id,ValidStatus,"completed");
    }

    private async Task<ActionResult<AppointmentDetailsDto>> UpdateAppointmentStatusAsync(int appointmentId,List<string> validStatus,string newStatus)
    {
        var doctor=await GetCurrentDoctorAsync();
        if(doctor is null) return Unauthorized();
        var appointment=await context.Appointments.Include(x=>x.Doctor).Include(x=>x.Patient)
                                                    .FirstOrDefaultAsync(x=>x.Id==appointmentId && x.DoctorId==doctor.Id);
        if(appointment is null) return NotFound();
        if(!validStatus.Any(x=>x==appointment.Status)) return BadRequest();
        appointment.Status=newStatus;
        await context.SaveChangesAsync();
        return Ok(appointment.ToDto());
    }
}