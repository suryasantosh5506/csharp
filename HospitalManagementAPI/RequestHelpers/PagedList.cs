using HospitalManagementAPI.Dtos.Doctor;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.RequestHelpers;

public class PagedList<T> : List<T>
{
    public PaginationMetaData Metadata { get; set; }

    public PagedList(PaginationMetaData paginationMetaData, List<T> items)
    {
        AddRange(items);
        Metadata=paginationMetaData;
    }

    public static async Task<PagedList<T>> ToPagedList(IQueryable<T> query,int currentPage,int pageSize)
    {
        int count=await query.CountAsync();
        int totalPages=(int)Math.Ceiling(count/(double)pageSize);
        PaginationMetaData metadata = new()
        {
            CurrentPage=currentPage,
            NoOfPages=totalPages,
            PageSize=pageSize,
            TotalDocuments=count,
        };
        var items=await query.Skip((currentPage-1)*pageSize).Take(pageSize).ToListAsync();
        return new PagedList<T>(metadata,items);
    }
}