namespace JobPortal.Dtos.Company;

public record CompanyDto
(
    int Id,
    string Name,
    string Description,
    string Website,
    string LogoUrl,
    int UserId,
    string UserName
);