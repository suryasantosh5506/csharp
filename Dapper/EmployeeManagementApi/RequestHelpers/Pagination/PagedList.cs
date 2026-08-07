namespace EmployeeManagementApi.RequestHelpers.Pagination;

public class PagedList<T>
{
    public PaginationMetaData paginationMetaData{get;}
    public IEnumerable<T> items{get;}

    public PagedList(IEnumerable<T>items,PaginationMetaData paginationMetaData)
    {
        this.items=items;
        this.paginationMetaData=paginationMetaData;
    }

    public static PagedList<T> ToPagedList(IEnumerable<T>items,int Totalcount,int PageNumber,int PageSize)
    {
        int TotalPages=(int)Math.Ceiling(Totalcount/(double)PageSize);
        PaginationMetaData metaData = new()
        {
            PageNumber=PageNumber,
            PageSize=PageSize,
            TotalCount=Totalcount,
            TotalPages=TotalPages,
        };

        return new PagedList<T>(items,metaData);
    }
}