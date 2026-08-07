namespace EmployeeManagementApi.RequestHelpers.Pagination;

public class PaginationParams
{
    private int Limit=10;
    private int _pageSize = 10;
    public int PageSize{
        get=>_pageSize;
        set=>_pageSize=(value<=Limit)?value:Limit;
    }
    public int PageNumber {get;set;}=1;
    
}