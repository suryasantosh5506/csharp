using EmployeeManagementApi.Dtos.Company;

namespace EmployeeManagementApi.Interfaces;

public interface ICompanyService
{
    Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync();

    Task<CompanyDto?> GetCompanyByIdAsync(int id);

    Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto createCompanyDto);

    Task<bool> UpdateCompanyAsync(int id, UpdateCompanyDto updateCompanyDto);

    Task<bool> DeleteCompanyAsync(int id);

    Task<CompanyDetailsDto> GetCompanyDetailsAsync(int id);

    Task<CompanyCompleteDto> GetCompanyCompleteAsync(int id);
}