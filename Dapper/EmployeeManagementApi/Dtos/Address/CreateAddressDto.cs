using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementApi.Dtos.Address;

public record CreateAddressDto(
    [Required]
    int EmployeeId,
    [Required]
    string HouseNo,
    [Required]
    string Street,
    [Required]
    string City,
    [Required]
    string State,
    [Required]
    string Country,
    [Required]
    string PostalCode
);