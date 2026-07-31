using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Doctor;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.Extensions;
using HospitalManagementAPI.Interfaces;
using HospitalManagementAPI.RequestHelpers;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Services;

public class DoctorService(HospitalContext context) : IDoctorService
{
    public async Task<PagedList<DoctorDetailsDto>> GetAllDoctorsAsync(PaginationParams paginationParams)
    {
        var query = context.Doctors.Include(x => x.Department).Select(x => x.ToDto());

        return await PagedList<DoctorDetailsDto>.ToPagedList(query,paginationParams.pageNumber,paginationParams.pageSize);
    }

    public async Task<DoctorDetailsDto?> GetDoctorByIdAsync(int id)
    {
        var doctor = await context.Doctors.Include(x => x.Department).FirstOrDefaultAsync(x => x.Id == id);
        return doctor?.ToDto();
    }

    public async Task<DoctorDetailsDto> CreateDoctorAsync(CreateDoctorDto dto)
    {
        Doctor doctor = new()
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim(),
            PhoneNumber = dto.PhoneNumber.Trim(),
            Qualification = dto.Qualification.Trim(),
            Specialization = dto.Specialization.Trim(),
            ExperienceYears = dto.ExperienceYears,
            ConsultationFee = dto.ConsultationFee,
            LicenseNumber = dto.LicenseNumber.Trim(),
            DepartmentId = dto.DepartmentId
        };

        context.Doctors.Add(doctor);
        await context.SaveChangesAsync();
        await context.Entry(doctor).Reference(x => x.Department).LoadAsync();
        return doctor.ToDto();
    }

    public async Task<DoctorDetailsDto?> UpdateDoctorAsync(int id, UpdateDoctorDto dto)
    {
        var doctor = await context.Doctors.Include(x => x.Department).FirstOrDefaultAsync(x => x.Id == id);

        if (doctor is null) return null;

        doctor.FirstName = dto.FirstName.Trim();
        doctor.LastName = dto.LastName.Trim();
        doctor.Email = dto.Email.Trim();
        doctor.PhoneNumber = dto.PhoneNumber.Trim();
        doctor.Qualification = dto.Qualification.Trim();
        doctor.Specialization = dto.Specialization.Trim();
        doctor.ExperienceYears = dto.ExperienceYears;
        doctor.ConsultationFee = dto.ConsultationFee;
        doctor.LicenseNumber = dto.LicenseNumber.Trim();
        doctor.DepartmentId = dto.DepartmentId;

        await context.SaveChangesAsync();
        await context.Entry(doctor).Reference(x => x.Department).LoadAsync();
        return doctor.ToDto();
    }

    public async Task<bool> DeleteDoctorAsync(int id)
    {
        var doctor = await context.Doctors.FindAsync(id);
        if (doctor is null) return false;
        context.Doctors.Remove(doctor);
        await context.SaveChangesAsync();
        return true;
    }
}