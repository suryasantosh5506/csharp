using System.Security.Claims;
using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Appointment;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.enums;
using HospitalManagementAPI.Interfaces;
using HospitalManagementAPI.RequestHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Controllers;

[Authorize(Roles =nameof(UserRole.Doctor))]
public class DoctorAppointmentController(
    HospitalContext context,
    IDoctorAppointmentService doctorAppointmentService) : BaseApiController
{
    private async Task<Doctor?> GetCurrentDoctorAsync()
    {
        var userId=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        return await context.Doctors.FirstOrDefaultAsync(x=>x.UserId==userId);
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<AppointmentDetailsDto>>> GetAllAppointmentsAsync([FromQuery]DoctorParams doctorParams)
    {
        var doctor=await GetCurrentDoctorAsync();

        if(doctor is null)
            return Unauthorized();

        var appointments=await doctorAppointmentService.GetAllAppointmentsAsync(doctor.Id,doctorParams);

        return Ok(appointments);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppointmentDetailsDto>> GetAppointmentByIdAsync(int id)
    {
        var doctor=await GetCurrentDoctorAsync();

        if(doctor is null)
            return Unauthorized();

        var appointment=await doctorAppointmentService.GetAppointmentByIdAsync(id,doctor.Id);

        if(appointment is null)
            return NotFound();

        return Ok(appointment);
    }

    [HttpPatch("{id:int}/approve")]
    public async Task<ActionResult<AppointmentDetailsDto>> ApproveAppointmentAsync(int id)
    {
        var doctor=await GetCurrentDoctorAsync();

        if(doctor is null)
            return Unauthorized();

        var appointment=await doctorAppointmentService.ApproveAppointmentAsync(id,doctor.Id);

        if(appointment is null)
            return BadRequest();

        return Ok(appointment);
    }

    [HttpPatch("{id:int}/reject")]
    public async Task<ActionResult<AppointmentDetailsDto>> RejectAppointmentAsync(int id)
    {
        var doctor=await GetCurrentDoctorAsync();

        if(doctor is null)
            return Unauthorized();

        var appointment=await doctorAppointmentService.RejectAppointmentAsync(id,doctor.Id);

        if(appointment is null)
            return BadRequest();

        return Ok(appointment);
    }

    [HttpPatch("{id:int}/complete")]
    public async Task<ActionResult<AppointmentDetailsDto>> CompleteAppointmentAsync(int id)
    {
        var doctor=await GetCurrentDoctorAsync();

        if(doctor is null)
            return Unauthorized();

        var appointment=await doctorAppointmentService.CompleteAppointmentAsync(id,doctor.Id);

        if(appointment is null)
            return BadRequest();

        return Ok(appointment);
    }
}