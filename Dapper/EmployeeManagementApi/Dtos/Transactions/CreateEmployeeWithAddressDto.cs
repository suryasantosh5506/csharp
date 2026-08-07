using EmployeeManagementApi.Dtos.Employee;

namespace EmployeeManagementApi.Dtos.Transactions;

public record CreateEmployeeWithAddressDto(
    CreateEmployeeDto Employee,
    CreateEmployeeAddressDto Address
);