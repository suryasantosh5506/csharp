using JobManagementApi.Dtos.Company;
using JobManagementApi.Entities;
using JobManagementApi.RequestHelpers.Pagination;

namespace JobManagementApi.Interfaces;

public interface ICompanyService
{
    Task<CompanyDto> CreateCompany(CreateCompanyDto dto);
    Task<PagedList<CompanyDto>> GetCompanies(PaginationParams paginationParams);
    Task<CompanyDto> GetCompanyById(int id);
    Task<bool> UpdateCompany(int id, UpdateCompanyDto dto);
    Task<bool> DeleteCompany(int id);
}