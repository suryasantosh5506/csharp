namespace EmployeeManagementApi.Dtos.Transactions;

public record CreateEmployeeAddressDto(
    string HouseNo,
    string Street,
    string City,
    string State,
    string Country,
    string PostalCode
);