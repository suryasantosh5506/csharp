namespace HospitalManagementAPI.RequestHelpers;

public class PaginationParams
{
    private int maxLimit=15;
    public int pageSize
    {
        get=>pageSize;
        set
        {
            pageSize=(value>maxLimit)?maxLimit:value;    
        }
    }

    public int pageNumber {get;set;}=1;

}