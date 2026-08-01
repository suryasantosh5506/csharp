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

[Authorize(Roles =nameof(UserRole.Patient))]
public class AppointmentController(HospitalContext context,IAppointmentService appointmentService) : BaseApiController
{
    private async Task<Patient?> GetCurrentPatientAsync()
    {
        var userId=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await context.Patients.FirstOrDefaultAsync(x=>x.UserId==userId);
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<AppointmentDetailsDto>>> GetAllAppointmentsAsync([FromQuery]PaginationParams paginationParams)
    {
        var patient=await GetCurrentPatientAsync();
        if(patient is null) return Unauthorized("Patient profile not found.");

        var appointments=await appointmentService.GetAllAppointmentsAsync(patient.Id,paginationParams);

        return Ok(appointments);
    }

    [HttpGet("{id:int}",Name="GetAppointmentById")]
    public async Task<ActionResult<AppointmentDetailsDto>> GetAppointmentById(int id)
    {
        var patient=await GetCurrentPatientAsync();
        if(patient is null) return Unauthorized("Patient profile not found.");

        var appointment=await appointmentService.GetAppointmentByIdAsync(id,patient.Id);

        if(appointment is null) return NotFound();

        return Ok(appointment);
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentDetailsDto>> CreateAppointmentAsync(CreateAppointmentDto newAppointment)
    {
        var patient=await GetCurrentPatientAsync();
        if(patient is null) return Unauthorized("Patient profile not found.");

        if(!await context.Doctors.AnyAsync(x=>x.Id==newAppointment.DoctorId))
            return BadRequest("Doctor not found.");

        if(newAppointment.AppointmentDate<DateOnly.FromDateTime(DateTime.Today))
            return BadRequest("Appointment date cannot be in the past.");

        bool doctorConflict=await context.Appointments.AnyAsync(x=>
            x.DoctorId==newAppointment.DoctorId &&
            x.AppointmentDate==newAppointment.AppointmentDate &&
            x.AppointmentTime==newAppointment.AppointmentTime);

        if(doctorConflict)
            return Conflict("Doctor is busy at the selected time.");

        bool patientConflict=await context.Appointments.AnyAsync(x=>
            x.PatientId==patient.Id &&
            x.AppointmentDate==newAppointment.AppointmentDate &&
            x.AppointmentTime==newAppointment.AppointmentTime);

        if(patientConflict)
            return Conflict("You already have another appointment at that time.");

        var appointment=await appointmentService.CreateAppointmentAsync(patient.Id,newAppointment);

        return CreatedAtRoute("GetAppointmentById",new{id=appointment.Id},appointment);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AppointmentDetailsDto>> UpdateAppointmentAsync(int id,UpdateAppointmentDto updateAppointmentDto)
    {
        var patient=await GetCurrentPatientAsync();
        if(patient is null) return Unauthorized("Patient profile not found.");

        if(updateAppointmentDto.AppointmentDate<DateOnly.FromDateTime(DateTime.Today))
            return BadRequest("Appointment date cannot be in the past.");

        var existingAppointment=await appointmentService.GetAppointmentByIdAsync(id,patient.Id);

        if(existingAppointment is null)
            return NotFound();

        bool doctorConflict=await context.Appointments.AnyAsync(x=>
            x.DoctorId==existingAppointment.DoctorId &&
            x.AppointmentDate==updateAppointmentDto.AppointmentDate &&
            x.AppointmentTime==updateAppointmentDto.AppointmentTime &&
            x.Id!=id);

        if(doctorConflict)
            return Conflict("Doctor is busy at the selected time.");

        bool patientConflict=await context.Appointments.AnyAsync(x=>
            x.PatientId==patient.Id &&
            x.AppointmentDate==updateAppointmentDto.AppointmentDate &&
            x.AppointmentTime==updateAppointmentDto.AppointmentTime &&
            x.Id!=id);

        if(patientConflict)
            return Conflict("You already have another appointment at that time.");

        var appointment=await appointmentService.UpdateAppointmentAsync(id,patient.Id,updateAppointmentDto);

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