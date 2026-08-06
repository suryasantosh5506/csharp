namespace EmployeeManagementApi.Dtos.Company;

public record CompanyDto(
    int Id,
    String Name,
    String Email,
    String Phone
);