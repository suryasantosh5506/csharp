using EmployeeManagementApi.Dtos.Company;
using EmployeeManagementApi.Entities;

namespace EmployeeManagementApi.Extensions;

public static class CompanyExtension{
    public static CompanyDto ToDto(this Company company)
    {
        return new(company.Id,
            company.Name,
            company.Email,
            company.Phone);
    }
}