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

[Authorize(Roles = nameof(UserRole.Patient))]
public class AppointmentController(HospitalContext context,IAppointmentService appointmentService) : BaseApiController
{
    private async Task<Patient?> GetCurrentPatientAsync()
    {
        var userId=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await context.Patients.FirstOrDefaultAsync(x=>x.UserId==userId);
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<AppointmentDetailsDto>>> GetAllAppointmentsAsync([FromQuery] DoctorParams doctorParams)
    {
        var patient=await GetCurrentPatientAsync();
        if(patient is null) return Unauthorized("Patient profile not found.");
        var appointments=await appointmentService.GetAllAppointmentsAsync(patient.Id,doctorParams);
        return Ok(appointments);
    }

    [HttpGet("{id:int}",Name ="GetAppointmentById")]
    public async Task<ActionResult<AppointmentDetailsDto>> GetAppointmentByIdAsync(int id)
    {
        var patient=await GetCurrentPatientAsync();
        if(patient is null) return Unauthorized("Patient profile not found.");
        var appointment=await appointmentService.GetAppointmentByIdAsync(id,patient.Id);
        if(appointment is null) return NotFound();
        return Ok(appointment);
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentDetailsDto>> CreateAppointmentAsync(CreateAppointmentDto createAppointmentDto)
    {
        var patient=await GetCurrentPatientAsync();
        if(patient is null) return Unauthorized("Patient profile not found.");
        var appointment=await appointmentService.CreateAppointmentAsync(patient.Id,createAppointmentDto);
        if(appointment is null)return BadRequest();
        return CreatedAtRoute("GetAppointmentById",new { id=appointment.Id },appointment);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AppointmentDetailsDto>> UpdateAppointmentAsync(int id,UpdateAppointmentDto updateAppointmentDto)
    {
        var patient=await GetCurrentPatientAsync();
        if(patient is null) return Unauthorized("Patient profile not found.");
        var appointment=await appointmentService.UpdateAppointmentAsync(id,patient.Id,updateAppointmentDto);
        if(appointment is null) return BadRequest();
        return Ok(appointment);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteAppointmentAsync(int id)
    {
        var patient=await GetCurrentPatientAsync();
        if(patient is null) return Unauthorized("Patient profile not found.");
        var deleted=await appointmentService.DeleteAppointmentAsync(id,patient.Id);
        if(!deleted) return NotFound();
        return NoContent();
    }
}