using HospitalManagementAPI.Dtos.Appointment;
using HospitalManagementAPI.RequestHelpers;

namespace HospitalManagementAPI.Services;

public interface IAdminAppointmentService
{
    Task<PagedList<AppointmentDetailsDto>> GetAllAppointmentsForAdminAsync(PaginationParams paginationParams);

    Task<AppointmentDetailsDto?> GetAppointmentByIdForAdminAsync(int id);

    Task<PagedList<AppointmentDetailsDto>> GetPendingAppointmentsAsync(PaginationParams paginationParams);
}