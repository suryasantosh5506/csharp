namespace LibraryManagementAPI.Models;

public class Book{
    public int Id{get;set;}
    public required string Title{get;set;}
    public decimal Price{get;set;}
    public DateOnly PublishedDate{get;set;}
    public int Stock{get;set;}
    public int AuthorId {get;set;}
    public Author Author{get;set;}
    public int CategoryId{get;set;}
    public Category Category{get;set;}
}

