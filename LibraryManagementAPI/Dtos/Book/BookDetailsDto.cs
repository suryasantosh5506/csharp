using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.Dtos.Book;

public record BookDetailsDto(
    int Id,
    [Required]
    [StringLength(50)]
    string Title,
    [Range(1,100)]
    decimal Price,
    DateOnly PublishedDate,
    [Range(1,100)]
    int Stock,
    [Required]
    [StringLength(50)]
    string Author,
    [Required]
    [StringLength(50)]
    string Category
);