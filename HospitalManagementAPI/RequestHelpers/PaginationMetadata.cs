namespace HospitalManagementAPI.RequestHelpers;

public class PaginationMetaData
{
    public int CurrentPage {get;set;}
    public int PageSize {get;set;}
    public int TotalDocuments {get;set;}
    public int NoOfPages {get;set;}
}