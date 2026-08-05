using JobPortal.Dtos.Company;
using JobPortal.Interfaces;
using LearnHubApi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.Controllers;

public class CompanyController(ICompanyService companyService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<CompanyDto>>> GetAllCompanies()
    {
        return Ok(await companyService.GetAllCompanies());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CompanyDto>> GetCompanyById(int id)
    {
        return Ok(await companyService.GetCompanyById(id));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CompanyDto>> CreateCompany(CreateCompanyDto dto)
    {
        return Ok(await companyService.CreateCompany(dto));
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CompanyDto>> UpdateCompany(int id, UpdateCompanyDto dto)
    {
        return Ok(await companyService.UpdateCompany(id, dto));
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteCompany(int id)
    {
        await companyService.DeleteCompany(id);
        return NoContent();
    }
}