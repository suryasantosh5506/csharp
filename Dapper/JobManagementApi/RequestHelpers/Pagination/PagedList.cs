namespace JobManagementApi.RequestHelpers.Pagination;

public class PagedList<T>
{
    public PaginationMetaData PaginationMetaData{get;set;}
    public IEnumerable<T> Items{get;set;}
    public PagedList(IEnumerable<T>items,PaginationMetaData paginationMetaData)
    {
        PaginationMetaData=paginationMetaData;
        Items=items;
    }

    public static PagedList<T> ToPagedList(IEnumerable<T>items,int pageNumber,int totalCount,int pageSize)
    {
        PaginationMetaData metaData = new()
        {
            PageNumber=pageNumber,
            PageSize=pageSize,
            TotalCount=totalCount,
            TotalPages=(int)Math.Ceiling(totalCount/(double)pageSize)
        };
        return new(items,metaData);
    }
}