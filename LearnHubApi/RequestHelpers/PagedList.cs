using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace LearnHubApi.RequestHelpers;

public class PagedList<T> : List<T>
{
    [JsonIgnore]
    public PaginationMetaData paginationMetaData {get;}
    public PagedList(List<T>items,PaginationMetaData paginationMetaData)
    {
        this.paginationMetaData=paginationMetaData;
        AddRange(items);
    }

    public static async Task<PagedList<T>> ToPagedList(IQueryable<T> query,int pageNumber,int pageSize)
    {
        var count=await query.CountAsync();
        int totalPages=(int)Math.Ceiling(count/(double)pageSize);
        PaginationMetaData metaData=new PaginationMetaData()
        {
            PageNumber=pageNumber,
            PageSize=pageSize,
            TotalCount=count,
            TotalPages=totalPages
        };
        List<T>items=await query.Skip((pageNumber-1)*pageSize).Take(pageSize).ToListAsync();
        return new PagedList<T>(items,metaData);
    }
}