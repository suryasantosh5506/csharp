using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementApi.Dtos.Address;

public record UpdateAddressDto(
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