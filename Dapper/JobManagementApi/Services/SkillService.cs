using System.Data;
using Dapper;
using JobManagementApi.Data;
using JobManagementApi.Dtos.Skills;
using JobManagementApi.Entities;
using JobManagementApi.Enums;
using JobManagementApi.Exceptions;
using JobManagementApi.Extensions;
using JobManagementApi.Interfaces;
using JobManagementApi.RequestHelpers.Pagination;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JobManagementApi.Services;

public class SkillService(DapperContext context,ICurrentUserService currentUser,ILogger<SkillService>logger) : ISkillService
{
    public async Task<SkillDto> CreateSkillAsync(CreateSkillDto dto)
    {
        if (!currentUser.IsAuthenticated)
        {
            logger.LogWarning("Someone tries to create a skill without proper authentication");
            throw new UnauthorizedException("Unauthorized");
        }
        if (currentUser.Role != UserRole.Admin)
        {
            logger.LogWarning($"User {currentUser.UserId} tried to create a skill without proper permissions");
            throw new ForbiddenException("You don't have access to perform this operation");
        }
        using var connection=context.GetConnection();
        var parameters=new {p_Name=dto.Name.Trim().ToLower()};
        string query="select * from skills where name=@p_Name";
        Skills? skill=await connection.QueryFirstOrDefaultAsync<Skills?>(query,parameters);
        if(skill is not null)
        {
            logger.LogWarning($"Admin {currentUser.UserId} tried to create already existing skill");
            throw new ConflictException("Skill alreay exists");
        }
        int rowsaffected=await connection.ExecuteAsync("CreateSkill",parameters,commandType:CommandType.StoredProcedure);
        if (rowsaffected == 0)
        {
            logger.LogCritical("Skill creation failed:Database responded with 0 rows affected");
            throw new Exception("Internal Server Error");
        }
        skill=await connection.QueryFirstAsync<Skills>(query,parameters);
        logger.LogInformation($"skill {skill.Id} created successfully");
        return skill.ToDto();
    }

    public async Task<bool> DeleteSkillAsync(int id)
    {
        if (!currentUser.IsAuthenticated)
        {
            logger.LogWarning("Unauthorized person tried to delete a skill");
            throw new UnauthorizedException("Unauthorized");
        }
        if (currentUser.Role != UserRole.Admin)
        {
            logger.LogWarning($"Forbidden:User {currentUser.UserId} tried to delete a skill without proper permission");
            throw new ForbiddenException("You don't have access to perform this operation");
        }
        using var connection=context.GetConnection();
        var parameters=new{p_Id=id};
        Skills? skill=await connection.QueryFirstOrDefaultAsync<Skills?>("GetSkillById",new{p_Id=id},commandType:CommandType.StoredProcedure);
        if(skill is null)
        {
            logger.LogWarning($"Admin {currentUser.UserId} tried to delete an non existing skill");
            throw new NotFoundException("skill not found");
        }
        int rowsaffected=await connection.ExecuteAsync("DeleteSkill",parameters,commandType:CommandType.StoredProcedure);
        if (rowsaffected == 0)
        {
            logger.LogCritical("Skill deletion failed: Database responded with 0 row affected");
            throw new Exception("Internal Server Error");
        }
        logger.LogInformation($"skill {id} deleted successfully");
        return true;
    }

    public async Task<PagedList<SkillDto>> GetAllSkillsAsync(PaginationParams paginationParams)
    {
        using var connection=context.GetConnection();
        string query="select * from skills order by id asc limit @limit offset @offset ";
        var parameters = new
        {
            limit=paginationParams.PageSize,
            offset=(paginationParams.PageNumber-1)*paginationParams.PageSize
        };

        int totalCount=await connection.QuerySingleAsync<int>("select count(*) from skills");
        var skills=await connection.QueryAsync<Skills>(query,parameters);
        return PagedList<SkillDto>.ToPagedList(skills.Select(x=>x.ToDto()),paginationParams.PageNumber,totalCount,paginationParams.PageSize);
    }

    public async Task<SkillDto> GetSkillAsync(int id)
    {
        using var connection=context.GetConnection();
        Skills? skill=await connection.QueryFirstOrDefaultAsync<Skills?>("GetSkillById",new{p_Id=id},commandType:CommandType.StoredProcedure);
        if(skill is null)
        {
            if(currentUser.IsAuthenticated) logger.LogWarning($"user {currentUser.UserId} tried to access non existing skill");
            else logger.LogWarning("Someone tried to access non existing skill");;
            throw new NotFoundException("skill not found");
        }
        return skill.ToDto();
    }

    public async Task<bool> UpdateSkillAsync(int id,UpdateSkillDto dto)
    {
        if (!currentUser.IsAuthenticated)
        {
            logger.LogWarning($"Unauthrized person tried to update the skill with id {id}");
            throw new UnauthorizedException("Unauthorized");
        }
        if (currentUser.Role != UserRole.Admin)
        {
            logger.LogWarning($"User {currentUser.UserId} tried to update the skill without proper permission");
            throw new ForbiddenException("You don't have access to perform this operation");
        }
        using var connection=context.GetConnection();
        Skills? skill=await connection.QueryFirstOrDefaultAsync<Skills?>("GetSkillById",new{p_Id=id},commandType:CommandType.StoredProcedure);
        if(skill is null)
        {
            logger.LogWarning($"Admin {currentUser.UserId} tried to update an non existing skill");
            throw new NotFoundException("skill not found");
        }
        var parameters=new {
            p_Id=id,
            p_Name=dto.Name.Trim().ToLower()
        };
        string query="select * from skills where name=@p_Name and id<>@p_Id";
        skill=await connection.QueryFirstOrDefaultAsync<Skills?>(query,parameters);
        if(skill is not null)
        {
            logger.LogWarning($"Admin {currentUser.UserId} tried to update the skill with an already existing skill");
            throw new ConflictException("Skill alreay exists");
        }
        int rowsaffected=await connection.ExecuteAsync("UpdateSkill",parameters,commandType:CommandType.StoredProcedure);
        if (rowsaffected == 0)
        {
            logger.LogCritical("Skill deletion failed: Database responded with 0 rows affected");
            throw new Exception("Internal Server Error");
        }
        logger.LogInformation($"Skill {id} updated successfully");
        return true;
    }
}