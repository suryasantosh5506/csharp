using HospitalManagementAPI.RequestHelpers;
namespace HospitalManagementAPI.RequestHelpers;

public class DoctorParams : PaginationParams
{
    public string? SearchTerm { get; set; }

    public int? DepartmentId { get; set; }

    public string? Specialization { get; set; }

    public decimal? MinFee { get; set; }

    public decimal? MaxFee { get; set; }

    public int? MinExperience { get; set; }

    public string? SortBy { get; set; }
}