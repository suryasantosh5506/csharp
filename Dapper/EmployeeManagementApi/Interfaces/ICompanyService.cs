using EmployeeManagementApi.Dtos.Company;
using EmployeeManagementApi.RequestHelpers.Pagination;

namespace EmployeeManagementApi.Interfaces;

public interface ICompanyService
{
    Task<PagedList<CompanyDto>> GetAllCompaniesAsync(PaginationParams paginationParams);

    Task<CompanyDto?> GetCompanyByIdAsync(int id);

    Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto createCompanyDto);

    Task<bool> UpdateCompanyAsync(int id, UpdateCompanyDto updateCompanyDto);

    Task<bool> DeleteCompanyAsync(int id);

    Task<CompanyDetailsDto> GetCompanyDetailsAsync(int id);

    Task<CompanyCompleteDto> GetCompanyCompleteAsync(int id);

    Task<CompanySummaryDto> GetCompanySummaryAsync(int id);
}