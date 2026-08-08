namespace JobManagementApi.Dtos.Company;

public record CompanyDto(
    int Id,
    int UserId,
    string Name,
    string Description,
    string Location,
    string Website
);