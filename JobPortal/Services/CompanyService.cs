using JobPortal.Data;
using JobPortal.Dtos.Company;
using JobPortal.Entities;
using JobPortal.Enums;
using JobPortal.Exceptions;
using JobPortal.Extensions;
using JobPortal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Services;

public class CompanyService(ICurrentUserService userService,AppDbContext context) : ICompanyService
{
    public async Task<CompanyDto> CreateCompany(CreateCompanyDto dto)
    {
        if(!userService.IsAuthenticated) throw new UnauthorizedException("Unauthorized");
        if(userService.Role!=UserRole.Employer && userService.Role!=UserRole.Admin) 
            throw new ForbiddenException("Only Employer and admin can create a new company");
        if(await context.Companies.AnyAsync(x=>x.Name.ToLower()==dto.Name.Trim().ToLower()))
            throw new ConflictException("Company already exists");
        Company company = new Company()
        {
            Name=dto.Name,
            Description=dto.Description,
            Website=dto.Website,
            LogoUrl=dto.LogoUrl,
            UserId=userService.UserID
        };
        context.Companies.Add(company);
        await context.SaveChangesAsync();
        company=await context.Companies.Include(x=>x.User).FirstAsync(x=>x.Id==company.Id);
        return company.ToDto();
    }

    public async Task DeleteCompany(int id)
    {
        if(!userService.IsAuthenticated) throw new UnauthorizedException("Unauthorized");
        if(userService.Role!=UserRole.Employer && userService.Role!=UserRole.Admin) 
            throw new ForbiddenException("Only Employer or Admin can delete a company");
        var company=await context.Companies.Include(x=>x.User).FirstOrDefaultAsync(x=>x.Id==id);
        if(company is null) throw new NotFoundException("Company not found");
        if(company.UserId!=userService.UserID && userService.Role!=UserRole.Admin)
            throw new ForbiddenException("Only the creator or admin can delete the company");
        context.Companies.Remove(company);
        await context.SaveChangesAsync();
    }

    public async Task<List<CompanyDto>> GetAllCompanies()
    {
        return await context.Companies.Include(x=>x.User).Select(x=>x.ToDto()).ToListAsync();
    }

    public async Task<CompanyDto> GetCompanyById(int id)
    {
        var company=await context.Companies.Include(x=>x.User).FirstOrDefaultAsync(x=>x.Id==id);
        if(company is null) throw new NotFoundException("Company not found");
        return company.ToDto();
    }

    public async Task<CompanyDto> UpdateCompany(int id, UpdateCompanyDto dto)
    {
        if(!userService.IsAuthenticated) throw new UnauthorizedException("Unauthorized");
        if(userService.Role!=UserRole.Employer && userService.Role!=UserRole.Admin) 
            throw new ForbiddenException("Only Employer or Admin can update a company");

        if(await context.Companies.AnyAsync(x=>x.Name.ToLower()==dto.Name.Trim().ToLower()))
        {
            throw new ConflictException("Company already exists");
        }
        var company=await context.Companies.Include(x=>x.User).FirstOrDefaultAsync(x=>x.Id==id);
        if(company is null) throw new NotFoundException("Company not found");
        if(company.UserId!=userService.UserID && userService.Role!=UserRole.Admin)
            throw new ForbiddenException("Only the creator or admin can update the company");

        company.Name=dto.Name;
        company.Description=dto.Description;
        company.Website=dto.Website;
        company.LogoUrl=dto.LogoUrl;
        await context.SaveChangesAsync();
        return company.ToDto();
    }
}