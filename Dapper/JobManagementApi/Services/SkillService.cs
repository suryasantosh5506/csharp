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

public class SkillService(DapperContext context,ICurrentUserService currentUser) : ISkillService
{
    public async Task<SkillDto> CreateSkillAsync(CreateSkillDto dto)
    {
        if(!currentUser.IsAuthenticated) throw new UnauthorizedException("Unauthorized");
        if(currentUser.Role!=UserRole.Admin) throw new ForbiddenException("You don't have access to perform this operation");
        using var connection=context.GetConnection();
        var parameters=new {p_Name=dto.Name.Trim().ToLower()};
        string query="select * from skills where name=@p_Name";
        Skills? skill=await connection.QueryFirstOrDefaultAsync<Skills?>(query,parameters);
        if(skill is not null) throw new ConflictException("Skill alreay exists");
        int rowsaffected=await connection.ExecuteAsync("CreateSkill",parameters,commandType:CommandType.StoredProcedure);
        if(rowsaffected==0) throw new Exception("Internal Server Error");
        skill=await connection.QueryFirstAsync<Skills>(query,parameters);
        return skill.ToDto();
    }

    public async Task<bool> DeleteSkillAsync(int id)
    {
        if(!currentUser.IsAuthenticated) throw new UnauthorizedException("Unauthorized");
        if(currentUser.Role!=UserRole.Admin) throw new Exception("You don't have access to perform this operation");
        using var connection=context.GetConnection();
        var parameters=new{p_Id=id};
        Skills? skill=await connection.QueryFirstOrDefaultAsync<Skills?>("GetSkillById",new{p_Id=id},commandType:CommandType.StoredProcedure);
        if(skill is null) throw new NotFoundException("skill not found");
        int rowsaffected=await connection.ExecuteAsync("DeleteSkill",parameters,commandType:CommandType.StoredProcedure);
        if(rowsaffected==0) throw new Exception("Internal Server Error");
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
        if(skill is null) throw new NotFoundException("skill not found");
        return skill.ToDto();
    }

    public async Task<bool> UpdateSkillAsync(int id,UpdateSkillDto dto)
    {
        if(!currentUser.IsAuthenticated) throw new UnauthorizedException("Unauthorized");
        if(currentUser.Role!=UserRole.Admin) throw new ForbiddenException("You don't have access to perform this operation");
        using var connection=context.GetConnection();
        Skills? skill=await connection.QueryFirstOrDefaultAsync<Skills?>("GetSkillById",new{p_Id=id},commandType:CommandType.StoredProcedure);
        if(skill is null) throw new NotFoundException("skill not found");
        var parameters=new {
            p_Id=id,
            p_Name=dto.Name.Trim().ToLower()
        };
        string query="select * from skills where name=@p_Name and id<>@p_Id";
        skill=await connection.QueryFirstOrDefaultAsync<Skills?>(query,parameters);
        if(skill is not null) throw new ConflictException("Skill alreay exists");
        int rowsaffected=await connection.ExecuteAsync("UpdateSkill",parameters,commandType:CommandType.StoredProcedure);
        if(rowsaffected==0) throw new Exception("Internal Server Error");
        return true;
    }
}