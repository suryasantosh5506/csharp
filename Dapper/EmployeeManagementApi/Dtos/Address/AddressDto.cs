namespace EmployeeManagementApi.Dtos.Address;

public record AddressDto(
    int Id,
    int EmployeeId,
    string HouseNo,
    string Street,
    string City,
    string State,
    string Country,
    string PostalCode
);