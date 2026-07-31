using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Patient;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.Extensions;
using HospitalManagementAPI.Interfaces;
using HospitalManagementAPI.RequestHelpers;

namespace HospitalManagementAPI.Services;

public class PatientService(HospitalContext context) : IPatientService
{
    public async Task<PatientDetailsDto> CreatePatientAsync(CreatePatientDto newPatient)
    {
        var patient=new Patient()
        {
            FirstName=newPatient.FirstName.Trim(),
            LastName=newPatient.LastName.Trim(),
            Email=newPatient.Email.Trim(),
            PhoneNumber=newPatient.PhoneNumber.Trim(),
            DateOfBirth=newPatient.DateOfBirth,
            Gender=newPatient.Gender.Trim(),
            BloodGroup=newPatient.BloodGroup.Trim(),
            Height=newPatient.Height,
            Weight=newPatient.Weight,
            Address=newPatient.Address.Trim(),
            EmergencyContactName=newPatient.EmergencyContactName.Trim(),
            EmergencyContactPhone=newPatient.EmergencyContactPhone.Trim(),
        };
        context.Patients.Add(patient);
        await context.SaveChangesAsync();
        return patient.ToDto();
    }

    public async Task<bool> DeletePatientAsync(int id)
    {
        var patient=await context.Patients.FindAsync(id);
        if(patient is null) return false;
        context.Patients.Remove(patient);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<PagedList<PatientDetailsDto>> GetAllPatientsAsync(PaginationParams paginationParams)
    {
        var query=context.Patients.Select(x=>x.ToDto());
        var patients=await PagedList<PatientDetailsDto>.ToPagedList(query,paginationParams.pageNumber,paginationParams.pageSize);
        return patients;
    }

    public async Task<PatientDetailsDto?> GetPatientByIdAsync(int id)
    {
        var patient = await context.Patients.FindAsync(id);
        if(patient is null) return null;
        return patient.ToDto();
    }

    public async Task<PatientDetailsDto?> UpdatePatientAsync(int id, UpdatePatientDto updatePatientDto)
    {
        var patient=await context.Patients.FindAsync(id);

        if(patient is null) return null;

        patient.FirstName=updatePatientDto.FirstName.Trim();
        patient.LastName=updatePatientDto.LastName.Trim();
        patient.Email=updatePatientDto.Email.Trim();
        patient.PhoneNumber=updatePatientDto.PhoneNumber.Trim();
        patient.DateOfBirth=updatePatientDto.DateOfBirth;
        patient.Gender=updatePatientDto.Gender.Trim();
        patient.BloodGroup=updatePatientDto.BloodGroup.Trim();
        patient.Height=updatePatientDto.Height;
        patient.Weight=updatePatientDto.Weight;
        patient.Address=updatePatientDto.Address.Trim();
        patient.EmergencyContactName=updatePatientDto.EmergencyContactName.Trim();
        patient.EmergencyContactPhone=updatePatientDto.EmergencyContactPhone.Trim();

        await context.SaveChangesAsync();
        return patient.ToDto();
    }
}