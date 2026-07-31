namespace HospitalManagementAPI.RequestHelpers;

public class PaginationParams
{
    private const int maxLimit = 15;
    private int _pageSize = 5;

    public int pageSize
    {
        get => _pageSize;
        set
        {
            _pageSize = (value > maxLimit) ? maxLimit : value;
        }
    }

    public int pageNumber { get; set; } = 1;
}