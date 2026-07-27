using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.Dtos.Book;

public record CreateBookDto(
    [Required]
    [StringLength(50)]
    string Title,
    [Required]
    [Range(1,100)]
    decimal Price,
    DateOnly PublishedDate,
    [Required]
    [Range(1,100)]
    int Stock,
    [Required]
    int AuthorId,
    [Required]
    int CategoryId
);