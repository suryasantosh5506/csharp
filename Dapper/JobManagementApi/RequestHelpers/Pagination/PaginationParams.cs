using System.Reflection.Metadata.Ecma335;

namespace JobManagementApi.RequestHelpers.Pagination;

public class PaginationParams
{
    private const int MaxSize=10;
    private int _pageSize{get;set;}=MaxSize;
    public int PageSize
    {
        get=>_pageSize;
        set=>_pageSize=(value<=MaxSize)?value:MaxSize;
    }
    public int PageNumber{get;set;}=1;
}