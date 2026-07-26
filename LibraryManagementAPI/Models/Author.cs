namespace LibraryManagementAPI.Models;

public class Author
{
    public int Id{get;set;}
    public required string Name{get;set;}
    public required string Email{get;set;}
    public required string Country{get;set;}
    public List<Book> Books { get; set; } = [];
}