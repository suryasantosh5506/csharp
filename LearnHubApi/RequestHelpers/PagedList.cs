using Microsoft.EntityFrameworkCore;

namespace LearnHubApi.RequestHelpers;

public class PagedList<T> : List<T>
{
    private PaginationMetaData _paginationMetaData;
    public PagedList(List<T>items,PaginationMetaData paginationMetaData)
    {
        _paginationMetaData=paginationMetaData;
        AddRange(items);
    }

    public static async Task<PagedList<T>> ToPagedList(IQueryable<T> query,int pageNumber,int pageSize)
    {
        var count=await query.CountAsync();

    }
}