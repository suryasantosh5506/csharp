using EmployeeManagementApi.Dtos.Company;
using EmployeeManagementApi.Interfaces;
using EmployeeManagementApi.RequestHelpers.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementApi.Controllers;

public class CompanyController(ICompanyService companyService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PagedList<CompanyDto>>> GetCompanies([FromQuery]PaginationParams paginationParams)
    {
        var companies = await companyService.GetAllCompaniesAsync(paginationParams);
        return Ok(companies);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CompanyDto>> GetCompany(int id)
    {
        var company = await companyService.GetCompanyByIdAsync(id);
        return Ok(company);
    }

    [HttpPost]
    public async Task<ActionResult<CompanyDto>> CreateCompany(CreateCompanyDto createCompanyDto)
    {
        var company = await companyService.CreateCompanyAsync(createCompanyDto);

        return CreatedAtAction(
            nameof(GetCompany),
            new { id = company.Id },
            company);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateCompany(int id, UpdateCompanyDto updateCompanyDto)
    {
        await companyService.UpdateCompanyAsync(id, updateCompanyDto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteCompany(int id)
    {
        await companyService.DeleteCompanyAsync(id);
        return NoContent();
    }

    [HttpGet("{id:int}/details")]
    public async Task<ActionResult<CompanyDetailsDto>> GetCompanyDetails(int id)
    {
        var company = await companyService.GetCompanyDetailsAsync(id);
        return Ok(company);
    }

    [HttpGet("{id:int}/complete")]
    public async Task<ActionResult<CompanyCompleteDto>> GetCompanyComplete(int id)
    {
        var company = await companyService.GetCompanyCompleteAsync(id);
        return Ok(company);
    }
}