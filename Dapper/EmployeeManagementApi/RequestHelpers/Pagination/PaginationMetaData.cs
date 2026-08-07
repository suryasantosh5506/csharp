namespace EmployeeManagementApi.RequestHelpers.Pagination;

public class PaginationMetaData
{
    public int PageNumber{get;set;}
    public int PageSize{get;set;}
    public int TotalCount{get;set;}
    public int TotalPages{get;set;}
}