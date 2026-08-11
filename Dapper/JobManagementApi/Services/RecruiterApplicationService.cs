using System.Data;
using System.Security.AccessControl;
using System.Text;
using Dapper;
using JobManagementApi.Data;
using JobManagementApi.Dtos.RecruiterApplication;
using JobManagementApi.Entities;
using JobManagementApi.Enums;
using JobManagementApi.Exceptions;
using JobManagementApi.Extensions;
using JobManagementApi.Interfaces;
using JobManagementApi.RequestHelpers.Pagination;

namespace JobManagementApi.Services;

public class RecruiterApplicationService(DapperContext context,ICurrentUserService currentUser,ILogger<RecruiterApplicationService>logger) : IRecruiterApplicationService
{
    public async Task<RecruiterApplicationDto> CreateApplication(CreateRecruiterApplicationDto dto)
    {
        if (!currentUser.IsAuthenticated)
        {
            logger.LogWarning("Someone tried to apply for recruiter without proper permission");
            throw new UnauthorizedException("Unauthorized");
        }
        if (currentUser.Role != UserRole.Candidate)
        {
            logger.LogWarning($"Forbidden:user {currentUser.UserId} tried to apply for recruiter without proper permission");
            throw new ForbiddenException("You don't have access to perform this operation");
        }
        using var connection=context.GetConnection();

        var parameters = new
        {
            p_CandidateId=currentUser.UserId,
            p_Status=RecruiterApplicationStatus.Pending,
            p_Reason=dto.Reason.Trim()   
        };

        RecruiterApplication? application=await connection.QueryFirstOrDefaultAsync<RecruiterApplication?>("GetRecruiterApplicationsByCandidateId",parameters,commandType:CommandType.StoredProcedure);

        if(application is not null && application.Status == RecruiterApplicationStatus.Pending)
        {
            logger.LogWarning($"User {currentUser.UserId} tried to apply for recruiter again while an application is already in progress");
            throw new ConflictException("Already applied");
        }

        int rowsaffected=await connection.ExecuteAsync("CreateRecruiterApplication",parameters,commandType:CommandType.StoredProcedure);
        if (rowsaffected == 0)
        {
            logger.LogCritical("Application to recruiter failed:database responded with 0 rows affected");
            throw new Exception("Internal Server error");
        }
        application=await connection.QueryFirstAsync<RecruiterApplication>("GetRecruiterApplicationsByCandidateId",parameters,commandType:CommandType.StoredProcedure);
        logger.LogInformation($"USer {currentUser.UserId} Successfully appled to recruiter role");
        return application.ToDto();
    }

    public async Task<bool> DeleteApplication(int id)
    {
        if (!currentUser.IsAuthenticated)
        {
            logger.LogWarning("Someone tried to delete the application for recruiter without proper permission");
            throw new UnauthorizedException("Unauthorized");
        }
        if(currentUser.Role!=UserRole.Candidate && currentUser.Role != UserRole.Admin)
        {
            logger.LogWarning($"Forbidden:user {currentUser.UserId} tried to delete the recruiter application without proper permission");
            throw new ForbiddenException("You dont have access to perform this operation");
        }
        using var connection=context.GetConnection();
        var parameters=new{p_Id=id};
        var application=await connection.QueryFirstOrDefaultAsync<RecruiterApplication?>("GetRecruiterApplicationById",parameters,commandType:CommandType.StoredProcedure);
        if(application is null)
        {
            logger.LogWarning($"User {currentUser.UserId} tried to delete an non existing application for recruiter role");
            throw new NotFoundException("application not found");
        }
        if(application.CandidateId!=currentUser.UserId && currentUser.Role != UserRole.Admin)
        {
            logger.LogWarning($"Forbidden:user {currentUser.UserId} tried to delete the recruiter application without proper permission");
            throw new ForbiddenException("You dont have access to perform this operation");
        }
        int rowsaffected=await connection.ExecuteAsync("DeleteRecruiterApplication",parameters,commandType:CommandType.StoredProcedure);
        if (rowsaffected == 0)
        {
            logger.LogCritical("Deletion of recruiter application failed:database responded with 0 rows affected");
            throw new Exception("Internal Server error");
        }
        logger.LogInformation("Application to recruiter role deleted successfully");
        return true;
    }

    public async Task<RecruiterApplicationDto> GetApplicationById(int id)
    {
        if(!currentUser.IsAuthenticated) throw new UnauthorizedException("Unauthorized");
        if(currentUser.Role!=UserRole.Candidate && currentUser.Role != UserRole.Admin)
        {
            throw new ForbiddenException("You don't have permission to perform this operation");
        }
        using var connection=context.GetConnection();
        var parameters=new {p_Id=id};
        var application=await connection.QueryFirstOrDefaultAsync<RecruiterApplication>("GetRecruiterApplicationById",parameters,commandType:CommandType.StoredProcedure);
        if(application is null) throw new NotFoundException("application not found");
        if(application.CandidateId!=currentUser.UserId && currentUser.Role != UserRole.Admin)
        {
            throw new ForbiddenException("You don't have permission to perform this operation");
        }
        return application.ToDto();
    }

    public async Task<PagedList<RecruiterApplicationDto>> GetApplications(PaginationParams paginationParams)
    {
        if(!currentUser.IsAuthenticated) throw new UnauthorizedException("Unauthorized");
        if(currentUser.Role!=UserRole.Admin) throw new ForbiddenException("You don't have permission to perform this operation");
        StringBuilder query=new();
        query.Append("select * from recruiterapplications order by id asc ");
        query.Append("limit @limit offset @offset ");
        using var connection=context.GetConnection();

        var parameters = new
        {
            limit=paginationParams.PageSize,
            offset=(paginationParams.PageNumber-1)*paginationParams.PageSize
        };

        int totalCount=await connection.QuerySingleAsync<int>("select count(*)  from recruiterapplications");

        var applications=await connection.QueryAsync<RecruiterApplication>(query.ToString(),parameters);
        return PagedList<RecruiterApplicationDto>.ToPagedList(applications.Select(x=>x.ToDto()),paginationParams.PageNumber,totalCount,paginationParams.PageSize);
    }

    public async Task<PagedList<RecruiterApplicationDto>> GetMyApplications(PaginationParams paginationParams)
    {
        if(!currentUser.IsAuthenticated) throw new UnauthorizedException("Unauthorized");
        if(currentUser.Role!=UserRole.Candidate) throw new ForbiddenException("You don't have permission to perform this operation");
        using var connection=context.GetConnection();
        
        var parameters = new
        {
            id=currentUser.UserId,
            limit=paginationParams.PageSize,
            offset=(paginationParams.PageNumber-1)*paginationParams.PageSize
        };

        StringBuilder query=new();
        query.Append("select * from recruiterapplications where CandidateId=@id order by id asc ");
        query.Append("limit @limit offset @offset ");

        int totalCount=await connection.QuerySingleAsync<int>("select count(*)  from recruiterapplications where CandidateId=@id",parameters);
        var application=await connection.QueryAsync<RecruiterApplication>(query.ToString(),parameters);

        return PagedList<RecruiterApplicationDto>.ToPagedList(application.Select(x=>x.ToDto()), paginationParams.PageNumber,
                        totalCount, paginationParams.PageSize);
    }

    public async Task<bool> UpdateApplication(int id, UpdateRecruiterApplicationDto dto)
    {
        if(!currentUser.IsAuthenticated) throw new UnauthorizedException("Unauthorized");
        if(currentUser.Role!=UserRole.Admin) throw new ForbiddenException("You don't have permission to perform this operation");
        if(dto.Status==RecruiterApplicationStatus.Pending) throw new BadRequestException("Invalid application status");
        using var connection=context.GetConnection();
        var parameters=new{
            p_Id=id,
            p_Status=dto.Status.ToString(),
            p_ReviewedBy=currentUser.UserId
        };
        var application=await connection.QueryFirstOrDefaultAsync<RecruiterApplication?>("GetRecruiterApplicationById",parameters,commandType:CommandType.StoredProcedure);
        if(application is null) throw new NotFoundException("application not found");
        if(application.Status!=RecruiterApplicationStatus.Pending) throw new ConflictException("Application has already been reviewed");

        int rowsaffected=0;
        if (dto.Status == RecruiterApplicationStatus.Approved)
        {
            connection.Open();
            using var transaction=connection.BeginTransaction();
            try
            {
                rowsaffected=await connection.ExecuteAsync("UpdateRecruiterApplication",parameters,commandType:CommandType.StoredProcedure,transaction:transaction);
                if(rowsaffected==0) throw new Exception("Internal server error");
                var approvedParameters = new
                {
                    role=UserRole.Recruiter.ToString(),
                    id=application.CandidateId
                };
                var query="update user set role=@role where id=@id";
                rowsaffected=await connection.ExecuteAsync(query,approvedParameters,transaction:transaction);
                if(rowsaffected==0) throw new Exception("Internal server error");
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }


        rowsaffected=await connection.ExecuteAsync("UpdateRecruiterApplication",parameters,commandType:CommandType.StoredProcedure);
        if(rowsaffected==0) throw new Exception("Internal server error");
        return true;
    }
}