using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Appointment;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.enums;
using HospitalManagementAPI.Extensions;
using HospitalManagementAPI.Interfaces;
using HospitalManagementAPI.RequestHelpers;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Services;

public class AppointmentService(HospitalContext context) : IAppointmentService
{
    public async Task<PagedList<AppointmentDetailsDto>> GetAllAppointmentsAsync(int patientId, PaginationParams paginationParams)
    {
        var query=context.Appointments.Where(x=>x.PatientId==patientId).Include(x=>x.Doctor).Include(x=>x.Patient).Select(x=>x.ToDto());

        return await PagedList<AppointmentDetailsDto>.ToPagedList(query,paginationParams.pageNumber,paginationParams.pageSize);
    }

    public async Task<AppointmentDetailsDto?> GetAppointmentByIdAsync(int id,int patientId)
    {
        var appointment=await context.Appointments.Include(x=>x.Doctor).Include(x=>x.Patient)
                                                    .FirstOrDefaultAsync(x=>x.Id==id && x.PatientId==patientId);

        if(appointment is null) return null;
        return appointment.ToDto();
    }

    public async Task<AppointmentDetailsDto?> CreateAppointmentAsync(int patientId,CreateAppointmentDto createAppointmentDto)
    {
        if(createAppointmentDto.AppointmentDate<DateOnly.FromDateTime(DateTime.Today))return null;

        bool doctorExists=await context.Doctors.AnyAsync(x=>x.Id==createAppointmentDto.DoctorId);

        if(!doctorExists) return null;

        bool doctorAvailable=await context.DoctorAvailabilities.AnyAsync(x=>
            x.DoctorId==createAppointmentDto.DoctorId &&
            x.DayOfWeek==createAppointmentDto.AppointmentDate.DayOfWeek &&
            x.IsAvailable &&
            createAppointmentDto.AppointmentTime>=x.StartTime &&
            createAppointmentDto.AppointmentTime<x.EndTime);

        if(!doctorAvailable) return null;

        bool doctorConflict=await context.Appointments.AnyAsync(x=>
            x.DoctorId==createAppointmentDto.DoctorId &&
            x.AppointmentDate==createAppointmentDto.AppointmentDate &&
            x.AppointmentTime==createAppointmentDto.AppointmentTime);

        if(doctorConflict) return null;

        bool patientConflict=await context.Appointments.AnyAsync(x=>
            x.PatientId==patientId &&
            x.AppointmentDate==createAppointmentDto.AppointmentDate &&
            x.AppointmentTime==createAppointmentDto.AppointmentTime);

        if(patientConflict) return null;

        Appointment appointment=new()
        {
            DoctorId=createAppointmentDto.DoctorId,
            PatientId=patientId,
            AppointmentDate=createAppointmentDto.AppointmentDate,
            AppointmentTime=createAppointmentDto.AppointmentTime,
            Reason=createAppointmentDto.Reason.Trim(),
            Status=AppointmentStatus.Pending,
            CreatedAt=DateTime.UtcNow
        };

        context.Appointments.Add(appointment);

        await context.SaveChangesAsync();

        appointment=await context.Appointments.Include(x=>x.Doctor).Include(x=>x.Patient).FirstAsync(x=>x.Id==appointment.Id);

        return appointment.ToDto();
    }

    public async Task<AppointmentDetailsDto?> UpdateAppointmentAsync(int id,int patientId,UpdateAppointmentDto updateAppointmentDto)
    {
        var appointment=await context.Appointments.Include(x=>x.Doctor).Include(x=>x.Patient)
                                                    .FirstOrDefaultAsync(x=>x.Id==id && x.PatientId==patientId);

        if(appointment is null) return null;

        if(updateAppointmentDto.AppointmentDate<DateOnly.FromDateTime(DateTime.Today)) return null;

        bool doctorAvailable=await context.DoctorAvailabilities.AnyAsync(x=>
                                                                        x.DoctorId==appointment.DoctorId &&
                                                                        x.DayOfWeek==updateAppointmentDto.AppointmentDate.DayOfWeek &&
                                                                        x.IsAvailable &&
                                                                        updateAppointmentDto.AppointmentTime>=x.StartTime &&
                                                                        updateAppointmentDto.AppointmentTime<x.EndTime);

        if(!doctorAvailable) return null;

        bool doctorConflict=await context.Appointments.AnyAsync(x=>
                                                                x.DoctorId==appointment.DoctorId &&
                                                                x.AppointmentDate==updateAppointmentDto.AppointmentDate &&
                                                                x.AppointmentTime==updateAppointmentDto.AppointmentTime &&
                                                                x.Id!=id);

        if(doctorConflict)
            return null;

        bool patientConflict=await context.Appointments.AnyAsync(x=>
                                                                x.PatientId==patientId &&
                                                                x.AppointmentDate==updateAppointmentDto.AppointmentDate &&
                                                                x.AppointmentTime==updateAppointmentDto.AppointmentTime &&
                                                                x.Id!=id);

        if(patientConflict) return null;

        appointment.AppointmentDate=updateAppointmentDto.AppointmentDate;
        appointment.AppointmentTime=updateAppointmentDto.AppointmentTime;
        appointment.Reason=updateAppointmentDto.Reason.Trim();
        appointment.Status=updateAppointmentDto.Status;

        await context.SaveChangesAsync();
        return appointment.ToDto();
    }

    public async Task<bool> DeleteAppointmentAsync(int id,int patientId)
    {
        var appointment=await context.Appointments.FirstOrDefaultAsync(x=>x.Id==id && x.PatientId==patientId);
        if(appointment is null)return false;
        context.Appointments.Remove(appointment);
        await context.SaveChangesAsync();
        return true;
    }
}