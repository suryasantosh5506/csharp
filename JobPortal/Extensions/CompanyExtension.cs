using JobPortal.Dtos.Company;
using JobPortal.Entities;

namespace JobPortal.Extensions;

public static class CompanyExtension
{
    public static CompanyDto ToDto(this Company company)
    {
        return new CompanyDto(
            company.Id,
            company.Name,
            company.Description,
            company.Website,
            company.LogoUrl,
            company.UserId,
            company.User.FullName
        );
    }
}