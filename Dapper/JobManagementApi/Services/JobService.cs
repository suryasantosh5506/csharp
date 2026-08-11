using System.Data;
using System.Diagnostics;
using System.Text;
using Dapper;
using JobManagementApi.Data;
using JobManagementApi.Dtos.Jobs;
using JobManagementApi.Entities;
using JobManagementApi.Enums;
using JobManagementApi.Exceptions;
using JobManagementApi.Extensions;
using JobManagementApi.Interfaces;
using JobManagementApi.RequestHelpers.Pagination;
using JobManagementApi.RequestHelpers.Searching;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.OpenApi;

namespace JobManagementApi.Services;

public class JobService(DapperContext context,ICurrentUserService currentUser,ILogger<JobService>logger) : IJobService
{
    public async Task<JobDto> CreateJob(CreateJobDto dto)
    {
        if (!currentUser.IsAuthenticated)
        {
            logger.LogWarning("Someone tried to create a job without proper authentication");
            throw new UnauthorizedException("Unauthorized");
        }
        if(currentUser.Role!=UserRole.Recruiter && currentUser.Role != UserRole.Admin)
        {
            logger.LogWarning($"Forbidden:User {currentUser.UserId} tried to create a job without proper permission");
            throw new ForbiddenException("only recruiter and admin can create a job");
        }
        using var connection=context.GetConnection();
        Company? company=await connection.QueryFirstOrDefaultAsync<Company?>("GetCompanyById",new{p_Id=dto.CompanyId},
        commandType: CommandType.StoredProcedure);
        if(company is null)
        {
            logger.LogWarning($"Forbidden:User {currentUser.UserId} tried to create a job for non-existing company");
            throw new NotFoundException("company not found");
        }
        if (company.UserId != currentUser.UserId && currentUser.Role != UserRole.Admin)
        {
            logger.LogWarning($"Forbidden:User {currentUser.UserId} tried to create a job without proper permission");   
            throw new ForbiddenException("You do not have permission to create a job for this company");
        }
        string query="select * from job where title=@p_Title and companyId=@p_CompanyId";
        var parameters=new{
            p_CompanyId=dto.CompanyId,
            p_RecruiterId=currentUser.UserId,
            p_Title=dto.Title.Trim().ToLower(),
            p_Description=string.IsNullOrEmpty(dto.Description)?string.Empty:dto.Description,
            p_Location=dto.Location,
            p_SalaryMin=dto.SalaryMin,
            p_SalaryMax=dto.SalaryMax,
            p_JobType=dto.JobType.ToString(),
            p_Experience=dto.Experience
        };
        Job? existJob=await connection.QueryFirstOrDefaultAsync<Job?>(query,parameters);
        if(existJob is not null)
        {
            logger.LogWarning($"User {currentUser.UserId} tried to create a job with already existing title");
            throw new ConflictException("Job with associated title already exists");
        }
        if (dto.SalaryMin > dto.SalaryMax)
        {
            logger.LogWarning($"User {currentUser.UserId} tried to create a job with incorrect information");
            throw new BadRequestException("Minimum salary cannot be greater than maximum salary");
        }

        if (dto.Experience < 0)
        {
            logger.LogWarning($"User {currentUser.UserId} tried to create a job with incorrect information");
            throw new BadRequestException("Experience cannot be negative");
        }
        int rowsaffected=await connection.ExecuteAsync("InsertJob",parameters,commandType: CommandType.StoredProcedure);
        if (rowsaffected == 0)
        {
            logger.LogCritical("Job creation failed:Database responded with 0 rows affected");
            throw new Exception("Internal Server Error");
        }
        Job job=await connection.QueryFirstAsync<Job>(query,parameters);
        logger.LogInformation($"Job {job.Id} created successfully");
        return job.ToDto();
    }

    public async Task<bool> DeleteJob(int id)
    {
        if (!currentUser.IsAuthenticated)
        {
            logger.LogWarning("Someone tried to delete a job without proper authentication");
            throw new UnauthorizedException("Unauthorized");
        }
        if(currentUser.Role!=UserRole.Admin && currentUser.Role != UserRole.Recruiter)
        {
            logger.LogWarning($"Forbidden:User {currentUser.UserId} tried to delete the job without proper permission");
            throw new ForbiddenException("only recruiter and admin can delete a job");
        }
        using var connection=context.GetConnection();
        Job? job=await connection.QueryFirstOrDefaultAsync<Job?>("GetJobById",new{p_Id=id},commandType:CommandType.StoredProcedure);
        if(job is null)
        {
            logger.LogWarning($"User {currentUser.UserId} tried to delete a non existing job");
            throw new NotFoundException("Job not found");
        }
        if(job.RecruiterId!=currentUser.UserId && currentUser.Role != UserRole.Admin)
        {
            logger.LogWarning($"Forbidden:User {currentUser.UserId} tried to delete a job without proper permission");
            throw new ForbiddenException("Only the owner and admin can delete a Job");
        }
        int rowsaffected=await connection.ExecuteAsync("DeleteJob",new{p_Id=id},commandType:CommandType.StoredProcedure);
        if (rowsaffected == 0)
        {
            logger.LogCritical("Job deletion failed:Database responded with 0 rows affected");
            throw new Exception("Internal Server Error");
        }
        logger.LogInformation("Job deleted successfully");
        return true;
    }

    public async Task<JobDto> GetJobById(int id)
    {
        using var connection=context.GetConnection();
        var job=await connection.QueryFirstOrDefaultAsync<Job>("GetJobById",new {p_Id=id},commandType:CommandType.StoredProcedure);
        if(job is null)
        {
            logger.LogWarning("Someone tried to access non-existing job");
            throw new NotFoundException("Job not found");
        }
        return job.ToDto();
    }

    public async Task<PagedList<JobDto>> GetJobs(JobParams jobParams)
    {
        using var connection=context.GetConnection();
        StringBuilder query=new();
        query.Append("select * from job ");

        StringBuilder conditions=new();

        var parameters=new DynamicParameters();
        parameters.Add("limit",jobParams.PageSize);
        parameters.Add("offset",(jobParams.PageNumber-1)*jobParams.PageSize);

        if (!string.IsNullOrWhiteSpace(jobParams.Search))
        {
            parameters.Add("search",$"%{jobParams.Search.ToLower().Trim()}%");
            conditions.Append("(title like @search or description like @search)");
        }

        if (!string.IsNullOrWhiteSpace(jobParams.Location))
        {
            parameters.Add("location",$"%{jobParams.Location.ToLower().Trim()}%");
            if(conditions.Length>0) conditions.Append(" and ");
            conditions.Append("location like @location");
        }

        if (jobParams.JobType.HasValue)
        {
            parameters.Add("jobtype",$"%{jobParams.JobType.ToString()}%");
            if(conditions.Length>0) conditions.Append(" and ");
            conditions.Append("jobtype like @jobtype");
        }

        if (jobParams.Experience.HasValue)
        {
            parameters.Add("experience",jobParams.Experience);
            if(conditions.Length>0) conditions.Append(" and ");
            conditions.Append("Experience<=@experience");
        }

        if (jobParams.Salary.HasValue)
        {
            parameters.Add("salary",jobParams.Salary);
            if(conditions.Length>0) conditions.Append(" and ");
            conditions.Append("salarymin<=@salary and salarymax>=@salary");
        }

        StringBuilder countQuery=new();
        countQuery.Append("select count(*) from job ");


        if(conditions.Length>0)
        {
            query.Append("where ");
            query.Append(conditions);
            countQuery.Append("where ");
            countQuery.Append(conditions);
        }

        string order=(jobParams.IsDescending)?"desc":"asc";
        if (!string.IsNullOrWhiteSpace(jobParams.SortBy))
        {
            query.Append(jobParams.SortBy.ToLower() switch
            {
                "companyid"=> $" order by companyid {order} ",
                "recruiterid"=>$" order by recruiterid {order} ",
                "title"=>$" order by title {order} ",
                "description"=>$" order by description {order} ",
                "location"=>$" order by location {order} ",
                "salarymin"=>$" order by salarymin {order} ",
                "salarymax"=>$" order by salarymax {order} ",
                "experience"=>$" order by experience {order} ",
                "createdat"=>$" order by createdat {order} ",
                _=>$" order by id {order} ",
            });
        }
        
        query.Append("limit @limit offset @offset ");

        int totalCount=await connection.ExecuteScalarAsync<int>(countQuery.ToString(),parameters);

        var jobs=await connection.QueryAsync<Job>(query.ToString(),parameters);
        return PagedList<JobDto>.ToPagedList(jobs.Select(x=>x.ToDto()),jobParams.PageNumber,totalCount,jobParams.PageSize);
    }

    public async Task<bool> UpdateJob(int id, UpdateJobDto dto)
    {
        if (!currentUser.IsAuthenticated)
        {
            logger.LogWarning("Someone tried to update the job without proper permission");
            throw new UnauthorizedException("Unauthorized");
        }
        if(currentUser.Role!=UserRole.Admin && currentUser.Role != UserRole.Recruiter)
        {
            logger.LogWarning($"Forbidden:User {currentUser.UserId} tried to update the job without proper permission");
            throw new ForbiddenException("only recruiter and admin can update a job");
        }

        if(dto.SalaryMin > dto.SalaryMax)
        {
            logger.LogWarning($"User {currentUser.UserId} tried to update the job with inappropriate data");
            throw new BadRequestException("Minimum salary cannot be greater than maximum salary");
        }

        if(dto.Experience < 0)
        {
            logger.LogWarning($"User {currentUser.UserId} tried to update the job with inappropriate data");
            throw new BadRequestException("Experience cannot be negative");
        }

        using var connection=context.GetConnection();

        Job? job=await connection.QueryFirstOrDefaultAsync<Job?>("GetJobById",new{p_Id=id},commandType:CommandType.StoredProcedure);
        if(job is null)
        {
            logger.LogWarning($"User {currentUser.UserId} tried to update the non-existng job");
            throw new NotFoundException("Job not found");
        }

        if(job.RecruiterId!=currentUser.UserId && currentUser.Role != UserRole.Admin)
        {
            logger.LogWarning($"Forbidden:User {currentUser.UserId} tried to update the job without proper permission");
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
        if(job is not null)
        {
            logger.LogWarning($"User {currentUser.UserId} tried to update the job with already existing title");
            throw new ConflictException("Job with specified title already exists");
        }

        int rowsaffected=await connection.ExecuteAsync("UpdateJob",parameters,commandType:CommandType.StoredProcedure);
        if (rowsaffected == 0)
        {
            logger.LogCritical("Job update failed:Database responded with 0 rows affected");
            throw new Exception("Internal Server Error");
        }
        logger.LogInformation($"Job {id} successfully updated");
        return true;
    }
}