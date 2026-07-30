using System.Security.Claims;
using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Appointment;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Controllers;

[Authorize(Roles = "Patient")]
public class AppointmentController(HospitalContext context) : BaseApiController
{
    private async Task<Patient?> GetCurrentPatientAsync()
    {
        var userId=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await context.Patients.FirstOrDefaultAsync(x=>x.UserId==userId);
    }

    [HttpGet]
    public async Task<ActionResult<List<AppointmentDetailsDto>>> GetAllAppointmentsAsync()
    {
        var patient=await GetCurrentPatientAsync();
        if(patient is null) return Unauthorized("Patient profile not found.");

        var appointments=await context.Appointments
            .Where(x=>x.PatientId==patient.Id)
            .Include(x=>x.Doctor)
            .Include(x=>x.Patient)
            .Select(x=>x.ToDto())
            .ToListAsync();

        return Ok(appointments);
    }

    [HttpGet("{id:int}",Name="GetAppointmentById")]
    public async Task<ActionResult<AppointmentDetailsDto>> GetAppointmentById(int id)
    {
        var patient=await GetCurrentPatientAsync();
        if(patient is null) return Unauthorized("Patient profile not found.");

        var appointment=await context.Appointments
            .Include(x=>x.Doctor)
            .Include(x=>x.Patient)
            .FirstOrDefaultAsync(x=>x.Id==id && x.PatientId==patient.Id);

        if(appointment is null) return NotFound();

        return Ok(appointment.ToDto());
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

        Appointment appointment=new()
        {
            DoctorId=newAppointment.DoctorId,
            PatientId=patient.Id,
            AppointmentDate=newAppointment.AppointmentDate,
            AppointmentTime=newAppointment.AppointmentTime,
            Reason=newAppointment.Reason.Trim(),
            Status="Pending",
            CreatedAt=DateTime.UtcNow
        };

        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();

        appointment=await context.Appointments
            .Include(x=>x.Doctor)
            .Include(x=>x.Patient)
            .FirstAsync(x=>x.Id==appointment.Id);

        return CreatedAtRoute("GetAppointmentById",new{id=appointment.Id},appointment.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AppointmentDetailsDto>> UpdateAppointmentAsync(int id,UpdateAppointmentDto updateAppointmentDto)
    {
        var patient=await GetCurrentPatientAsync();
        if(patient is null) return Unauthorized("Patient profile not found.");

        var appointment=await context.Appointments
            .Include(x=>x.Doctor)
            .Include(x=>x.Patient)
            .FirstOrDefaultAsync(x=>x.Id==id && x.PatientId==patient.Id);

        if(appointment is null) return NotFound();

        if(updateAppointmentDto.AppointmentDate<DateOnly.FromDateTime(DateTime.Today))
            return BadRequest("Appointment date cannot be in the past.");

        bool doctorConflict=await context.Appointments.AnyAsync(x=>
            x.DoctorId==appointment.DoctorId &&
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

        appointment.AppointmentDate=updateAppointmentDto.AppointmentDate;
        appointment.AppointmentTime=updateAppointmentDto.AppointmentTime;
        appointment.Reason=updateAppointmentDto.Reason.Trim();
        appointment.Status=updateAppointmentDto.Status.Trim();

        await context.SaveChangesAsync();

        return Ok(appointment.ToDto());
    }
}