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
    public async Task<PagedList<DoctorDetailsDto>> GetAllDoctorsAsync(DoctorParams doctorParams)
    {
        IQueryable<Doctor> query=context.Doctors.Include(x=>x.Department);

        if(!string.IsNullOrWhiteSpace(doctorParams.SearchTerm))
        {
            string search=doctorParams.SearchTerm.Trim().ToLower();

            query=query.Where(x=>
                x.FirstName.ToLower().Contains(search) ||
                x.LastName.ToLower().Contains(search) ||
                x.Specialization.ToLower().Contains(search) ||
                x.HospitalName.ToLower().Contains(search));
        }

        if(doctorParams.DepartmentId.HasValue)
        {
            query=query.Where(x=>x.DepartmentId==doctorParams.DepartmentId.Value);
        }

        if(!string.IsNullOrWhiteSpace(doctorParams.Specialization))
        {
            string specialization=doctorParams.Specialization.Trim().ToLower();
            query=query.Where(x=>x.Specialization.ToLower()==specialization);
        }

        if(doctorParams.MinFee.HasValue)
        {
            query=query.Where(x=>x.ConsultationFee>=doctorParams.MinFee.Value);
        }

        if(doctorParams.MaxFee.HasValue)
        {
            query=query.Where(x=>x.ConsultationFee<=doctorParams.MaxFee.Value);
        }

        if(doctorParams.MinExperience.HasValue)
        {
            query=query.Where(x=>x.ExperienceYears>=doctorParams.MinExperience.Value);
        }

        query=doctorParams.SortBy?.ToLower() switch
        {
            "name" => query.OrderBy(x=>x.FirstName).ThenBy(x=>x.LastName),

            "-name" => query.OrderByDescending(x=>x.FirstName)
                            .ThenByDescending(x=>x.LastName),

            "experience" => query.OrderBy(x=>x.ExperienceYears),

            "-experience" => query.OrderByDescending(x=>x.ExperienceYears),

            "fee" => query.OrderBy(x=>x.ConsultationFee),

            "-fee" => query.OrderByDescending(x=>x.ConsultationFee),

            _ => query.OrderBy(x=>x.FirstName)
        };

        return await PagedList<DoctorDetailsDto>.ToPagedList(
            query.Select(x=>x.ToDto()),
            doctorParams.pageNumber,
            doctorParams.pageSize);
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