namespace LearnHubApi.RequestHelpers;

public class PaginationParams
{
    public int PageNumber{get;set;}=1;
    private const int Limit=10;
    private int _pagesize=Limit;

    public int PageSize
    {
        get=>_pagesize;
        set =>_pagesize=(value<=Limit)?value:Limit;
        
    }
}