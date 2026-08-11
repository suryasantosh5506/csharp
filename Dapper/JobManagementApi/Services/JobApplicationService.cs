using System.Data;
using System.Text;
using Dapper;
using JobManagementApi.Data;
using JobManagementApi.Dtos.JobApplication;
using JobManagementApi.Entities;
using JobManagementApi.Enums;
using JobManagementApi.Exceptions;
using JobManagementApi.Extensions;
using JobManagementApi.Interfaces;
using JobManagementApi.RequestHelpers.Pagination;

namespace JobManagementApi.Services;

public class JobApplicationService(DapperContext context,ICurrentUserService currentUser,ILogger<JobApplicationService>logger) : IJobApplicationService
{
    public async Task<JobApplicationDto> CreateApplication(int jobId, CreateJobApplicationDto dto)
    {
        if (!currentUser.IsAuthenticated)
        {
            logger.LogWarning("Unauthorized access:SomeOne tried to create the job application without proper authentication");
            throw new UnauthorizedException("Unauthorized");
        }
        if (currentUser.Role != UserRole.Candidate)
        {
            logger.LogWarning($"user {currentUser.UserId} tries to apply to job without proper permission");
            throw new ForbiddenException("Only candidate can apply");
        }
        using var connection=context.GetConnection();
        Job? job=await connection.QueryFirstOrDefaultAsync<Job?>("GetJobById",new{p_Id=jobId},commandType:CommandType.StoredProcedure);
        if(job is null)
        {
            logger.LogWarning($"user {currentUser.UserId} tries to apply to non existing job");
            throw new NotFoundException("Job not found");
        }
        var parameters=new
        {
          p_Id=currentUser.UserId,
          p_JobId=jobId, 
          p_CandidateId=currentUser.UserId, 
          p_ResumeUrl=dto.ResumeUrl,
          p_Status=ApplicationStatus.Applied.ToString(),
        };
        string query="select * from application where JobId=@p_JobId and CandidateId=@p_CandidateId";
        Application? existingapplication=await connection.QueryFirstOrDefaultAsync<Application?>(query,parameters);
        
        if(existingapplication is not null)
        {
            logger.LogWarning($"user {currentUser.UserId} tries to apply the job with an application still under processing");
            throw new ConflictException("Already applied to this job");
        }


        int rowsaffected=await connection.ExecuteAsync("CreateApplication",parameters,commandType:CommandType.StoredProcedure);
        if (rowsaffected == 0)
        {
            logger.LogCritical("job application creation failed:Database responded with 0 rows affected");
            throw new Exception("Internal server error");
        }
        logger.LogInformation($"user {currentUser.UserId} Applied to job successfully");
        var jobApplication=await connection.QueryFirstAsync<Application>(query,parameters);
        return jobApplication.ToDto();
    }

    public async Task<bool> DeleteApplication(int id)
    {
        if (!currentUser.IsAuthenticated)
        {
            logger.LogWarning("Unauthorized access:SomeOne tried to delete the job application without proper authentication");
            throw new UnauthorizedException("Unauthorized");
        }
        if (currentUser.Role != UserRole.Candidate && currentUser.Role != UserRole.Admin)
        {
            logger.LogWarning($"user {currentUser.UserId} tries to apply to job without proper permission");
            throw new ForbiddenException("Only candidate and admin can delete an application");
        }
        var connection=context.GetConnection();

        Application? application=await connection.QueryFirstOrDefaultAsync<Application?>("GetJobApplicationById",new{p_Id=id},
                                        commandType:CommandType.StoredProcedure);
        if(application is null)
        {
            logger.LogWarning($"user {currentUser.UserId} tries to delete non existing application");
            throw new NotFoundException("application not found");
        }
        if (application.CandidateId != currentUser.UserId && currentUser.Role!=UserRole.Admin)
        {
            logger.LogWarning($"user {currentUser.UserId} tries to apply to job without proper permission");
            throw new ForbiddenException("Only applied candidate or admin have access to delete");
        }
        int rowsaffected=await connection.ExecuteAsync("DeleteApplication",new{p_Id=id},commandType:CommandType.StoredProcedure);
        if (rowsaffected == 0)
        {
            logger.LogCritical("job application Deletion failed:Database responded with 0 rows affected");
            throw new Exception("Internal server error");
        }
        logger.LogInformation($"Application {id} deleted successfully");
        return true;
    }

    public async Task<JobApplicationDto> GetApplicationById(int id)
    {
        if (!currentUser.IsAuthenticated)
        {
            logger.LogWarning("Unauthorized access:SomeOne tried to access the job application without proper authentication");
            throw new UnauthorizedException("Unauthorized");
        }
        using var connection=context.GetConnection();
        Application? application=await connection.QueryFirstOrDefaultAsync<Application?>("GetJobApplicationById",new{p_Id=id},commandType:CommandType.StoredProcedure);
        if(application is null)
        {
            logger.LogWarning($"User {currentUser.UserId} tried to access an non-existing job application");
            throw new NotFoundException("Application not found");
        }

        if (currentUser.Role == UserRole.Candidate && application.CandidateId!=currentUser.UserId)
        {
            logger.LogWarning($"Forbidden: User {currentUser.UserId} tried to access application {id} without proper permission");
            throw new ForbiddenException("You do not have access to this application");
        }
        if (currentUser.Role == UserRole.Recruiter)
        {
            var job=await connection.QueryFirstAsync<Job>("GetJobById",new{p_Id=application.JobId},commandType:CommandType.StoredProcedure);
            if (job.RecruiterId != currentUser.UserId)
            {
                logger.LogWarning($"Forbidden:User(Recruiter) tried to access job application {id} without proper permissions");
                throw new ForbiddenException("You do not have access to this application");
            }
        }
        return application.ToDto();
    }

    public async Task<PagedList<JobApplicationDto>> GetJobApplications(int jobId,PaginationParams paginationParams)
    {
        if (!currentUser.IsAuthenticated)
        {
            logger.LogWarning("Unauthorized access:SomeOne tried to access the job aplications without proper authentication");
            throw new UnauthorizedException("Unauthorized");
        }
        if(currentUser.Role!=UserRole.Recruiter && currentUser.Role != UserRole.Admin)
        {
            logger.LogWarning($"Forbidden:User {currentUser.UserId} tried to access the job application {jobId} without proper permission");
            throw new ForbiddenException("Only recruiter and admin can see the job applications");
        }
        using var connection=context.GetConnection();
        Job? job=await connection.QueryFirstOrDefaultAsync<Job?>("GetJobById",new{p_Id=jobId},commandType:CommandType.StoredProcedure);
        if(job is null)
        {
            logger.LogWarning($"User {currentUser.UserId} tried to access an non-existing job application");
            throw new NotFoundException("job not found");
        }
        if(job.RecruiterId!=currentUser.UserId && currentUser.Role != UserRole.Admin)
        {
            logger.LogWarning($"Forbidden:User {currentUser.UserId} tried to access the job application {jobId} without proper permission");
            throw new ForbiddenException("only admin and creator of job can view job applications");
        }

        StringBuilder query=new();
        
        query.Append("select * from application where jobid=@id order by id asc ");
        query.Append("limit @limit offset @offset");

        var parameters = new
        {
            id=jobId,
            limit=paginationParams.PageSize,
            offset=(paginationParams.PageNumber-1)*paginationParams.PageSize
        };

        int totalCount=await connection.ExecuteScalarAsync<int>("select count(*) from application where JobId=@id",parameters);

        var jobApplications=await connection.QueryAsync<Application>(query.ToString(),parameters);
        return PagedList<JobApplicationDto>.ToPagedList(jobApplications.Select(x=>x.ToDto()),paginationParams.PageNumber,totalCount,paginationParams.PageSize);
    }

    public async Task<PagedList<JobApplicationDto>> GetMyApplications(PaginationParams paginationParams)
    {
        if (!currentUser.IsAuthenticated)
        {
            logger.LogWarning("Unauthorized access:SomeOne tried to access the job applications without proper authentication");
            throw new UnauthorizedException("Unauthorized");
        }
        if (currentUser.Role != UserRole.Candidate)
        {
            logger.LogWarning($"Forbidden:User {currentUser.UserId} tried to access the job applications without proper permission");
            throw new ForbiddenException("Only candidate can access this route");
        }
        using var connection=context.GetConnection();

        StringBuilder query=new();

        query.Append("select * from application where candidateId=@id order by id asc ");
        query.Append("limit @limit offset @offset");

        var parameters = new
        {
            id=currentUser.UserId,
            limit=paginationParams.PageSize,
            offset=(paginationParams.PageNumber-1)*paginationParams.PageSize
        };

        int totalCount=await connection.ExecuteScalarAsync<int>("select count(*) from application where candidateId=@id",parameters);


        var jobApplications=await connection.QueryAsync<Application>(query.ToString(),parameters);
        return PagedList<JobApplicationDto>.ToPagedList(jobApplications.Select(x=>x.ToDto()),paginationParams.PageNumber,totalCount,paginationParams.PageSize);
    }

    public async Task<bool> UpdateApplicationStatus(int id, UpdateJobApplicationDto dto)
    {
        if (!currentUser.IsAuthenticated)
        {
            logger.LogWarning("Unauthorized access:SomeOne tried to update the job application without proper authentication");
            throw new UnauthorizedException("Unauthorized");
        }
        if (currentUser.Role != UserRole.Recruiter && currentUser.Role != UserRole.Admin)
        {
            logger.LogWarning($"Forbidden:User {currentUser.UserId} tried to update the job application {id} without proper permission");
            throw new ForbiddenException("Only recruiter and admin can delete an application");
        }
        var connection=context.GetConnection();
        var parameters = new
        {
            p_Id=id,
            p_Status=dto.Status.ToString()
        };
        
        var jobApplication=await connection.QueryFirstOrDefaultAsync<Application?>("GetJobApplicationById",parameters,
                            commandType:CommandType.StoredProcedure);
        if(jobApplication is null)
        {
            logger.LogWarning($"User {currentUser.UserId} tried to access non-existing job application");
            throw new NotFoundException("Application not found");
        }
        var job=await connection.QueryFirstAsync<Job>("GetJobById",new {p_Id=jobApplication.JobId},commandType:CommandType.StoredProcedure);
        if(job.RecruiterId!=currentUser.UserId && currentUser.Role != UserRole.Admin)
        {
            logger.LogWarning($"Forbidden:User {currentUser.UserId} tried to access the job application {id} without proper permission");
            throw new ForbiddenException("Doesn't have access to update this application");
        }
        int rowsaffected=await connection.ExecuteAsync("UpdateApplicationStatus",parameters,commandType:CommandType.StoredProcedure);
        if (rowsaffected == 0)
        {
            logger.LogCritical("Job application Deletion failed:Database responded with 0 rows affected");
            throw new Exception("Internal server error");
        }
        logger.LogInformation($"user {currentUser.UserId} updates the job application {id} successfully");
        return true;
    }
}