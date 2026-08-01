using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.DoctorAvailability;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.Extensions;
using HospitalManagementAPI.Interfaces;
using HospitalManagementAPI.RequestHelpers;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Services;

public class DoctorAvailabilityService(HospitalContext context) : IDoctorAvailabilityService
{
    public async Task<DoctorAvailabilityDetailsDto?> CreateAvailabilityAsync(int doctorId, CreateDoctorAvailabilityDto dto)
    {
        if(dto.StartTime>=dto.EndTime) return null;

        bool exists=await context.DoctorAvailabilities.AnyAsync(x=>x.DoctorId==doctorId && x.DayOfWeek==dto.DayOfWeek &&
                                                                    dto.StartTime<x.EndTime && dto.EndTime>x.StartTime);

        if(exists) return null;

        DoctorAvailability availability = new()
        {
            DoctorId=doctorId,
            DayOfWeek=dto.DayOfWeek,
            StartTime=dto.StartTime,
            EndTime=dto.EndTime,
            IsAvailable=true,
        };

        context.DoctorAvailabilities.Add(availability);

        await context.SaveChangesAsync();

        return availability.ToDto();
    }

    public async Task<bool> DeleteAvailabilityAsync(int id, int doctorId)
    {
        var availability=await context.DoctorAvailabilities
            .FirstOrDefaultAsync(x=>x.Id==id && x.DoctorId==doctorId);

        if(availability is null) return false;

        context.DoctorAvailabilities.Remove(availability);

        await context.SaveChangesAsync();

        return true;
    }

    public async Task<DoctorAvailabilityDetailsDto?> GetAvailabilityByIdAsync(int id, int doctorId)
    {
        var availability=await context.DoctorAvailabilities
            .FirstOrDefaultAsync(x=>x.Id==id && x.DoctorId==doctorId);

        if(availability is null) return null;

        return availability.ToDto();
    }

    public async Task<PagedList<DoctorAvailabilityDetailsDto>> GetDoctorAvailabilityAsync(int doctorId, PaginationParams paginationParams)
    {
        var query=context.DoctorAvailabilities
            .Where(x=>x.DoctorId==doctorId)
            .Select(x=>x.ToDto());

        return await PagedList<DoctorAvailabilityDetailsDto>.ToPagedList(
            query,
            paginationParams.pageNumber,
            paginationParams.pageSize);
    }

    public async Task<DoctorAvailabilityDetailsDto?> UpdateAvailabilityAsync(int id, int doctorId, UpdateDoctorAvailabilityDto dto)
    {
        var availability=await context.DoctorAvailabilities
            .FirstOrDefaultAsync(x=>x.Id==id && x.DoctorId==doctorId);

        if(availability is null) return null;

        if(dto.StartTime>=dto.EndTime) return null;

        bool exists=await context.DoctorAvailabilities.AnyAsync(x=>x.DoctorId==doctorId && x.DayOfWeek==dto.DayOfWeek && 
                                                                dto.StartTime<x.EndTime && dto.EndTime>x.StartTime &&
                                                                x.Id!=id);

        if(exists) return null;

        availability.DayOfWeek=dto.DayOfWeek;
        availability.StartTime=dto.StartTime;
        availability.EndTime=dto.EndTime;
        availability.IsAvailable=dto.IsAvailable;

        await context.SaveChangesAsync();

        return availability.ToDto();
    }
}