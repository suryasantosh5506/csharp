using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Appointment;
using HospitalManagementAPI.Extensions;
using HospitalManagementAPI.RequestHelpers;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Services;

public class AdminAppointmentService(HospitalContext context) : IAdminAppointmentService
{
    public async Task<PagedList<AppointmentDetailsDto>> GetAllAppointmentsForAdminAsync(PaginationParams paginationParams)
    {
        var query=context.Appointments
            .Include(x=>x.Patient)
            .Include(x=>x.Doctor)
            .Select(x=>x.ToDto());

        var appointments=await PagedList<AppointmentDetailsDto>.ToPagedList(
            query,
            paginationParams.pageNumber,
            paginationParams.pageSize);

        return appointments;
    }

    public async Task<AppointmentDetailsDto?> GetAppointmentByIdForAdminAsync(int id)
    {
        var appointment=await context.Appointments
            .Include(x=>x.Patient)
            .Include(x=>x.Doctor)
            .FirstOrDefaultAsync(x=>x.Id==id);

        if(appointment is null) return null;

        return appointment.ToDto();
    }

    public async Task<PagedList<AppointmentDetailsDto>> GetPendingAppointmentsAsync(PaginationParams paginationParams)
    {
        var query=context.Appointments
            .Include(x=>x.Patient)
            .Include(x=>x.Doctor)
            .Where(x=>x.Status=="pending")
            .Select(x=>x.ToDto());

        var appointments=await PagedList<AppointmentDetailsDto>.ToPagedList(
            query,
            paginationParams.pageNumber,
            paginationParams.pageSize);

        return appointments;
    }
}