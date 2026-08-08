namespace EmployeeManagementApi.Dtos.Company;

public record CompanySummaryDto(
    CompanyDetailsDto DetailsDto,
    int EmployeeCount
);