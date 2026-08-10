using JobManagementApi.Enums;
using JobManagementApi.RequestHelpers.Pagination;

namespace JobManagementApi.RequestHelpers.Searching;

public class JobParams:PaginationParams
{
    public string? Search{get;set;}
    public string? Location{get;set;}
    public JobTypes? JobType{get;set;}
    public int? Experience{get;set;}
    public decimal? Salary{get;set;}
    public string? SortBy{get;set;}="Id";
    public bool IsDescending{get;set;}=false;
}