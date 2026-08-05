using JobPortal.Dtos.Company;

namespace JobPortal.Interfaces;

public interface ICompanyService
{
    Task<CompanyDto> CreateCompany(CreateCompanyDto dto);

    Task<List<CompanyDto>> GetAllCompanies();

    Task<CompanyDto> GetCompanyById(int id);

    Task<CompanyDto> UpdateCompany(int id, UpdateCompanyDto dto);

    Task DeleteCompany(int id);
}