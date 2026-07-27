using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.Dtos.Category;


public record CategoryDetailsDto(
    int Id,
    [Required]
    [StringLength(50)]
    string Name
);