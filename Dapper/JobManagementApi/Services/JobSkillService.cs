using JobManagementApi.Data;
using JobManagementApi.Dtos.Skills;
using JobManagementApi.Exceptions;
using JobManagementApi.Interfaces;
using JobManagementApi.Enums;
using JobManagementApi.Entities;
using Dapper;
using System.Data;
using JobManagementApi.Extensions;

namespace JobManagementApi.Services;

public class JobSkillService(DapperContext context,ICurrentUserService currentUser) : IJobSkillService
{
    public async Task<bool> AddSkillToJob(int jobId, int skillId)
    {
        if(!currentUser.IsAuthenticated) throw new UnauthorizedException("Unauthorized");
        if(currentUser.Role!=UserRole.Admin && currentUser.Role!=UserRole.Recruiter){
            throw new ForbiddenException("You don't have access to perform this operation");
        }
        using var connection=context.GetConnection();
        var parameters=new {p_JobId=jobId,p_SkillId=skillId};
        Job? job=await connection.QueryFirstOrDefaultAsync<Job?>("GetJobById",new { p_Id = jobId },commandType:CommandType.StoredProcedure);
        if(job is null) throw new NotFoundException("Job Not Found");
        Skills? skill=await connection.QueryFirstOrDefaultAsync<Skills?>("GetSkillById",new{p_Id=skillId},commandType:CommandType.StoredProcedure);
        if(skill is null) throw new NotFoundException("Skill Not Found");
        if(job.RecruiterId!=currentUser.UserId && currentUser.Role!=UserRole.Admin)
        {
            throw new ForbiddenException("You do not have access to this job");
        }
        var query="select * from jobskills where jobId=@p_JobId and skillid=@p_SkillId";
        var exist=await connection.QueryFirstOrDefaultAsync<JobSkills?>(query,parameters);
        if(exist is not null) throw new ConflictException("Skill already added");
        int rowsaffected=await connection.ExecuteAsync("AddJobSkill",parameters,commandType:CommandType.StoredProcedure);
        if(rowsaffected==0) throw new Exception("Internal server Exception");
        return true;
    }

    public async Task<IEnumerable<SkillDto>> GetJobSkills(int jobId)
    {
        using var connection=context.GetConnection();
        Job? job=await connection.QueryFirstOrDefaultAsync<Job?>("GetJobById",new {p_Id=jobId},commandType:CommandType.StoredProcedure);
        if(job is null) throw new NotFoundException("Job Not Found");
        var skills=await connection.QueryAsync<Skills>("GetJobSkills",new{p_JobId=jobId},commandType:CommandType.StoredProcedure);
        return skills.Select(x=>x.ToDto());
    }

    public async Task<bool> RemoveSkillFromJob(int jobId, int skillId)
    {
        if(!currentUser.IsAuthenticated) throw new UnauthorizedException("Unauthorized");
        if(currentUser.Role!=UserRole.Admin && currentUser.Role!=UserRole.Recruiter){
            throw new ForbiddenException("You don't have access to perform this operation");
        }
        using var connection=context.GetConnection();
        var parameters=new {p_JobId=jobId,p_SkillId=skillId};
        Job? job=await connection.QueryFirstOrDefaultAsync<Job?>("GetJobById",new {p_Id=jobId},commandType:CommandType.StoredProcedure);
        if(job is null) throw new NotFoundException("Job Not Found");
        Skills? skill=await connection.QueryFirstOrDefaultAsync<Skills?>("GetSkillById",new{p_Id=skillId},commandType:CommandType.StoredProcedure);
        if(skill is null) throw new NotFoundException("Skill Not Found");
        if(job.RecruiterId!=currentUser.UserId && currentUser.Role!=UserRole.Admin)
        {
            throw new ForbiddenException("You do not have access to this job");
        }
        string query="select * from jobskills where jobId=@p_JobId and skillid=@p_SkillId";
        var exist=await connection.QueryFirstOrDefaultAsync<JobSkills?>(query,parameters);
        if(exist is null) throw new ConflictException("skill was not associated with this job");
        int rowsaffected=await connection.ExecuteAsync("RemoveJobSkill",parameters,commandType:CommandType.StoredProcedure);
        if(rowsaffected==0) throw new Exception("Internal server Exception");
        return true;
    }
}