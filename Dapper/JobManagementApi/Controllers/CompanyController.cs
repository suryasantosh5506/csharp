using JobManagementApi.Dtos.Company;
using JobManagementApi.Interfaces;
using JobManagementApi.RequestHelpers.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobManagementApi.Controllers;

public class CompanyController(ICompanyService companyService) : BaseApiController
{
    [HttpPost]
    [Authorize(Roles = "Admin,Recruiter")]
    public async Task<ActionResult<CompanyDto>> CreateCompany(
        CreateCompanyDto dto)
    {
        var company = await companyService.CreateCompany(dto);

        return Ok(company);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedList<CompanyDto>>> GetCompanies([FromQuery]PaginationParams paginationParams)
    {
        var companies = await companyService.GetCompanies(paginationParams);

        return Ok(companies);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<CompanyDto>> GetCompanyById(int id)
    {
        var company = await companyService.GetCompanyById(id);

        return Ok(company);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Recruiter")]
    public async Task<IActionResult> UpdateCompany(
        int id,
        UpdateCompanyDto dto)
    {
        await companyService.UpdateCompany(id, dto);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Recruiter")]
    public async Task<IActionResult> DeleteCompany(int id)
    {
        await companyService.DeleteCompany(id);

        return NoContent();
    }
}