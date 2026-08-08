using JobManagementApi.Dtos.Company;
using JobManagementApi.Entities;

namespace JobManagementApi.Interfaces;

public interface ICompanyService
{
    Task<CompanyDto> CreateCompany(CreateCompanyDto dto);
    Task<IEnumerable<CompanyDto>> GetCompanies();
    Task<CompanyDto> GetCompanyById(int id);
    Task<bool> UpdateCompany(int id, UpdateCompanyDto dto);
    Task<bool> DeleteCompany(int id);
}