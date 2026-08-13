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
            p_CandidateId = currentUser.UserId,
            p_Status=RecruiterApplicationStatus.Pending.ToString(),
            p_Reason=dto.Reason.Trim()   
        };

        var applications=(await connection.QueryAsync<RecruiterApplication>("GetRecruiterApplicationsByCandidateId",new{p_CandidateId=currentUser.UserId},
                            commandType:CommandType.StoredProcedure)).ToList();

        if(applications.Count!=0   && applications.Any(x=>x.Status==RecruiterApplicationStatus.Pending))
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
        var application=await connection.QueryFirstAsync<RecruiterApplication>("GetRecruiterApplicationsByCandidateId",new{p_CandidateId=currentUser.UserId},
                            commandType:CommandType.StoredProcedure);
        logger.LogInformation($"User {currentUser.UserId} Successfully appled to recruiter role");
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
        if (!currentUser.IsAuthenticated)
        {
            logger.LogWarning("Someone tried to access application for recruiter role without proper authentication");
            throw new UnauthorizedException("Unauthorized");
        }
        if(currentUser.Role!=UserRole.Candidate && currentUser.Role != UserRole.Admin)
        {
            logger.LogWarning($"Forbidden:User {currentUser.UserId} tried to access the recruiter application {id} without proper permission");
            throw new ForbiddenException("You don't have permission to perform this operation");
        }
        using var connection=context.GetConnection();
        var parameters=new {p_Id=id};
        var application=await connection.QueryFirstOrDefaultAsync<RecruiterApplication>("GetRecruiterApplicationById",parameters,commandType:CommandType.StoredProcedure);
        if(application is null)
        {
            logger.LogWarning($"user {currentUser.UserId} tried to access the non existing recruiter application");
            throw new NotFoundException("application not found");
        }
        if(application.CandidateId!=currentUser.UserId && currentUser.Role != UserRole.Admin)
        {
            logger.LogWarning($"Forbidden:User {currentUser.UserId} tried to access the recruiter application {id} without proper permission");
            throw new ForbiddenException("You don't have permission to perform this operation");
        }
        return application.ToDto();
    }

    public async Task<PagedList<RecruiterApplicationDto>> GetApplications(PaginationParams paginationParams)
    {
        if (!currentUser.IsAuthenticated)
        {
            logger.LogWarning("Someone tried to access all the recruiter applications without proper authentication");
            throw new UnauthorizedException("Unauthorized");
        }
        if (currentUser.Role != UserRole.Admin)
        {
            logger.LogWarning($"Forbidden:User {currentUser.UserId} tried to access the recruiter applications without proper permission");
            throw new ForbiddenException("You don't have permission to perform this operation");
        }
        StringBuilder query=new();
        query.Append("select * from recruiterapplications order by id asc ");
        query.Append("offset @offset rows fetch next @limit rows only ");
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
        if (!currentUser.IsAuthenticated)
        {
            logger.LogWarning("Someone tried to access previous recruiter applications without proper authentication");
            throw new UnauthorizedException("Unauthorized");
        }
        if (currentUser.Role != UserRole.Candidate)
        {
            logger.LogWarning($"user {currentUser.UserId} tried to access their previous recruiter applications without proper permission");
            throw new ForbiddenException("You don't have permission to perform this operation");
        }
        using var connection=context.GetConnection();
        
        var parameters = new
        {
            id=currentUser.UserId,
            limit=paginationParams.PageSize,
            offset=(paginationParams.PageNumber-1)*paginationParams.PageSize
        };

        StringBuilder query=new();
        query.Append("select * from recruiterapplications where CandidateId=@id order by id asc ");
        query.Append("offset @offset rows fetch next @limit rows only ");

        int totalCount=await connection.QuerySingleAsync<int>("select count(*)  from recruiterapplications where CandidateId=@id",parameters);
        var application=await connection.QueryAsync<RecruiterApplication>(query.ToString(),parameters);

        return PagedList<RecruiterApplicationDto>.ToPagedList(application.Select(x=>x.ToDto()), paginationParams.PageNumber,
                        totalCount, paginationParams.PageSize);
    }

    public async Task<bool> UpdateApplication(int id, UpdateRecruiterApplicationDto dto)
    {
        if (!currentUser.IsAuthenticated)
        {
            logger.LogWarning("Someone tried to update the application to recruiter role without proper authentication");
            throw new UnauthorizedException("Unauthorized");
        }
        if (currentUser.Role != UserRole.Admin)
        {
            logger.LogWarning($"Forbidden:user {currentUser.UserId} tried to update the application for recruiter role without proper permission");
            throw new ForbiddenException("You don't have permission to perform this operation");
        }
        if (dto.Status == RecruiterApplicationStatus.Pending)
        {
            logger.LogWarning($"Admin {currentUser.UserId} tried to update recruiter application {id} with invalid state");
            throw new BadRequestException("Invalid application status");
        }

        using var connection=context.GetConnection();

        var application=await connection.QueryFirstOrDefaultAsync<RecruiterApplication?>("GetRecruiterApplicationById",new {p_Id=id},commandType:CommandType.StoredProcedure);

        if(application is null)
        {
            logger.LogWarning($"Admin {currentUser.UserId} tries to update the status of an non-existing application");
            throw new NotFoundException("application not found");
        }

        if (application.Status != RecruiterApplicationStatus.Pending)
        {
            logger.LogWarning($"Admin {currentUser.UserId} tried to update the state of already processed recruiter application {id}");
            throw new ConflictException("Application has already been reviewed");
        }

        var parameters=new
        {
            p_Id=id,
            p_Status=dto.Status.ToString(),
            p_ReviewedBy=currentUser.UserId
        };

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

                var query="update Users set Role=@role where Id=@id";

                rowsaffected=await connection.ExecuteAsync(query,approvedParameters,transaction:transaction);

                if (rowsaffected == 0)
                {
                    logger.LogCritical("Updating the recruiter application status failed:database responded with 0 rows affected");
                    throw new Exception("Internal Server error");
                }

                transaction.Commit();
                logger.LogInformation($"Successfully updated the status of recruiter application with id {id}");
                return true;
            }
            catch
            {
                logger.LogCritical("Updating the status of recruiter application failed: database responded with 0 rows affected");
                transaction.Rollback();
                throw;
            }
        }

        rowsaffected=await connection.ExecuteAsync("UpdateRecruiterApplication",parameters,commandType:CommandType.StoredProcedure);

        if (rowsaffected == 0)
        {
            logger.LogCritical("Updating the status of recruiter application failed:database responded with 0 rows affected");
            throw new Exception("Internal Server error");
        }

        logger.LogInformation($"Successfully updated the status of recruiter application with id {id}");
        return true;
    }
}