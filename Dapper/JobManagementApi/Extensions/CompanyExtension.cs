using System.Security.AccessControl;
using JobManagementApi.Dtos.Company;
using JobManagementApi.Entities;

namespace JobManagementApi.Extensions;

public static class CompanyExtension
{
    public static CompanyDto ToDto(this Company company)
    {
        return new(company.Id,company.UserId,company.Name,company.Description,company.Location,company.Website);
    }
}