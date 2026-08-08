using EmployeeManagementApi.RequestHelpers.Pagination;
namespace EmployeeManagementApi.RequestHelpers.Search;

public class EmployeeParams : PaginationParams
{
    public string? SearchTerm { get; set; }

    public string? SortBy { get; set; }

    public bool IsDescending { get; set; }=false;

    public string? Email { get; set; }

    public string? Phone { get; set; }
}