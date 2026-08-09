using System.Data;
using System.Text;
using Dapper;
using JobManagementApi.Data;
using JobManagementApi.Dtos.Jobs;
using JobManagementApi.Entities;
using JobManagementApi.Enums;
using JobManagementApi.Exceptions;
using JobManagementApi.Extensions;
using JobManagementApi.Interfaces;

namespace JobManagementApi.Services;

public class JobService(DapperContext context,ICurrentUserService currentUser) : IJobService
{
    public async Task<JobDto> CreateJob(CreateJobDto dto)
    {
        if(!currentUser.IsAuthenticated) throw new UnauthorizedException("Unauthorized");
        if(currentUser.Role!=UserRole.Recruiter && currentUser.Role != UserRole.Admin)
        {
            throw new ForbiddenException("only recruiter and admin can create a job");
        }
        using var connection=context.GetConnection();
        Company? company=await connection.QueryFirstOrDefaultAsync<Company?>("GetCompanyById",new{p_Id=dto.CompanyId},
        commandType: CommandType.StoredProcedure);
        if(company is null) throw new NotFoundException("company not found");
        if (company.UserId != currentUser.UserId && currentUser.Role != UserRole.Admin)
        {
            throw new ForbiddenException("You do not have permission to create a job for this company");
        }
        string query="select * from job where title=@p_Title and companyId=@p_CompanyId";
        var parameters=new{
            p_Title=dto.Title.Trim().ToLower(),
            p_CompanyId=dto.CompanyId,
            p_RecruiterId=currentUser.UserId,
            p_Description=string.IsNullOrEmpty(dto.Description)?string.Empty:dto.Description,
            p_Location=dto.Location,
            p_SalaryMin=dto.SalaryMin,
            p_SalaryMax=dto.SalaryMax,
            p_JobType=dto.JobType.ToString(),
            p_Experience=dto.Experience
        };
        Job? existJob=await connection.QueryFirstOrDefaultAsync<Job?>(query,parameters);
        if(existJob is not null) throw new ConflictException("Job with associated title already exists");
        if (dto.SalaryMin > dto.SalaryMax)
        {
            throw new BadRequestException("Minimum salary cannot be greater than maximum salary");
        }

        if (dto.Experience < 0)
        {
            throw new BadRequestException("Experience cannot be negative");
        }
        int rowsaffected=await connection.ExecuteAsync("InsertJob",parameters,commandType: CommandType.StoredProcedure);
        if(rowsaffected==0) throw new Exception("Internal Server Error");
        Job job=await connection.QueryFirstAsync<Job>(query,parameters);
        return job.ToDto();
    }

    public async Task<bool> DeleteJob(int id)
    {
        if(!currentUser.IsAuthenticated) throw new UnauthorizedException("Unauthorized");
        if(currentUser.Role!=UserRole.Admin && currentUser.Role != UserRole.Recruiter)
        {
            throw new ForbiddenException("only recruiter and admin can create a job");
        }
        using var connection=context.GetConnection();
        Job? job=await connection.QueryFirstOrDefaultAsync<Job?>("GetJobById",new{p_Id=id},commandType:CommandType.StoredProcedure);
        if(job is null) throw new NotFoundException("Job not found");
        if(job.RecruiterId!=currentUser.UserId && currentUser.Role != UserRole.Admin)
        {
            throw new ForbiddenException("Only the owner and admin can delete a Job");
        }
        int rowsaffected=await connection.ExecuteAsync("DeleteJob",new{p_Id=id},commandType:CommandType.StoredProcedure);
        if(rowsaffected==0) throw new Exception("Internal Server Eroor");
        return true;
    }

    public async Task<JobDto> GetJobById(int id)
    {
        using var connection=context.GetConnection();
        var job=await connection.QueryFirstOrDefaultAsync<Job>("GetJobById",new {p_Id=id},commandType:CommandType.StoredProcedure);
        if(job is null) throw new NotFoundException("Job not found");
        return job.ToDto();
    }

    public async Task<IEnumerable<JobDto>> GetJobs()
    {
        using var connection=context.GetConnection();
        StringBuilder query=new();
        query.Append("select * from job");
        var jobs=await connection.QueryAsync<Job>(query.ToString());
        return jobs.Select(x=>x.ToDto());
    }

    public async Task<bool> UpdateJob(int id, UpdateJobDto dto)
    {
        if(!currentUser.IsAuthenticated) throw new UnauthorizedException("Unauthorized");
        if(currentUser.Role!=UserRole.Admin && currentUser.Role != UserRole.Recruiter)
        {
            throw new ForbiddenException("only recruiter and admin can update a job");
        }

        if(dto.SalaryMin > dto.SalaryMax)
        {
            throw new BadRequestException("Minimum salary cannot be greater than maximum salary");
        }

        if(dto.Experience < 0)
        {
            throw new BadRequestException("Experience cannot be negative");
        }

        using var connection=context.GetConnection();

        Job? job=await connection.QueryFirstOrDefaultAsync<Job?>("GetJobById",new{p_Id=id},commandType:CommandType.StoredProcedure);
        if(job is null) throw new NotFoundException("Job not found");

        if(job.RecruiterId!=currentUser.UserId && currentUser.Role != UserRole.Admin)
        {
            throw new ForbiddenException("Only the owner and admin can update a Job");
        }
        string query="Select * from job where Title=@p_Title and CompanyId=@p_CompanyId and Id<>@p_Id";

        var parameters = new
        {
            p_Id=id,
            p_CompanyId=job.CompanyId,
            p_Title=dto.Title.Trim().ToLower(),
            p_Description=string.IsNullOrEmpty(dto.Description)?string.Empty:dto.Description,
            p_Location=dto.Location,
            p_SalaryMin=dto.SalaryMin,
            p_SalaryMax=dto.SalaryMax,
            p_JobType=dto.JobType.ToString(),
            p_Experience=dto.Experience
        };

        job=await connection.QuerySingleOrDefaultAsync<Job?>(query,parameters);
        if(job is not null) throw new ConflictException("Job with specified title already exists");

        int rowsaffected=await connection.ExecuteAsync("UpdateJob",parameters,commandType:CommandType.StoredProcedure);
        if(rowsaffected==0) throw new Exception("Internal Server Error");
        return true;
    }
}