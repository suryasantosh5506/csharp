using JobManagementApi.RequestHelpers.Pagination;

namespace JobManagementApi.RequestHelpers.Searching;

public class CompanyParams:PaginationParams
{
    public string? Search{get;set;}
    public string? Location{get;set;}
}