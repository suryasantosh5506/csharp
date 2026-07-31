using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Patient;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.Extensions;
using HospitalManagementAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Services;

public class PatientProfileService(HospitalContext context) : IPatientProfileService
{
    public async Task<PatientDetailsDto?> GetProfileAsync(int userId)
    {
        var patient=await context.Patients.FirstOrDefaultAsync(x=>x.UserId==userId);

        if(patient is null) return null;

        return patient.ToDto();
    }

    public async Task<PatientDetailsDto> CreateProfileAsync(int userId,CreatePatientDto dto)
    {
        Patient patient=new()
        {
            UserId=userId,

            FirstName=dto.FirstName,
            LastName=dto.LastName,
            Email=dto.Email,
            PhoneNumber=dto.PhoneNumber,

            DateOfBirth=dto.DateOfBirth,
            Gender=dto.Gender,
            BloodGroup=dto.BloodGroup,
            Height=dto.Height,
            Weight=dto.Weight,
            Address=dto.Address,
            EmergencyContactName=dto.EmergencyContactName,
            EmergencyContactPhone=dto.EmergencyContactPhone
        };

        context.Patients.Add(patient);

        await context.SaveChangesAsync();

        return patient.ToDto();
    }

    public async Task<PatientDetailsDto?> UpdateProfileAsync(int userId,UpdatePatientDto dto)
    {
        var patient=await context.Patients.FirstOrDefaultAsync(x=>x.UserId==userId);

        if(patient is null) return null;

        patient.FirstName=dto.FirstName;
        patient.LastName=dto.LastName;
        patient.Email=dto.Email;
        patient.PhoneNumber=dto.PhoneNumber;

        patient.DateOfBirth=dto.DateOfBirth;
        patient.Gender=dto.Gender;
        patient.BloodGroup=dto.BloodGroup;
        patient.Height=dto.Height;
        patient.Weight=dto.Weight;
        patient.Address=dto.Address;
        patient.EmergencyContactName=dto.EmergencyContactName;
        patient.EmergencyContactPhone=dto.EmergencyContactPhone;

        await context.SaveChangesAsync();

        return patient.ToDto();
    }

    public async Task<bool> DeleteProfileAsync(int userId)
    {
        var patient=await context.Patients.FirstOrDefaultAsync(x=>x.UserId==userId);

        if(patient is null) return false;

        context.Patients.Remove(patient);

        await context.SaveChangesAsync();

        return true;
    }
}