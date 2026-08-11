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

public class JobSkillService(DapperContext context,ICurrentUserService currentUser,ILogger<JobSkillService>logger) : IJobSkillService
{
    public async Task<bool> AddSkillToJob(int jobId, int skillId)
    {
        if (!currentUser.IsAuthenticated)
        {
            logger.LogWarning("Someone tried to add skill to the job without proper authentication");
            throw new UnauthorizedException("Unauthorized");
        }
        if(currentUser.Role!=UserRole.Admin && currentUser.Role!=UserRole.Recruiter){
            logger.LogWarning($"Forbidden:user {currentUser.UserId} tries to add skills to job without proper permission");
            throw new ForbiddenException("You don't have access to perform this operation");
        }
        using var connection=context.GetConnection();
        var parameters=new {p_JobId=jobId,p_SkillId=skillId};
        Job? job=await connection.QueryFirstOrDefaultAsync<Job?>("GetJobById",new { p_Id = jobId },commandType:CommandType.StoredProcedure);
        if(job is null)
        {
            logger.LogWarning($"user {currentUser.UserId} tried to add skills to non-existing job");
            throw new NotFoundException("Job Not Found");
        }
        Skills? skill=await connection.QueryFirstOrDefaultAsync<Skills?>("GetSkillById",new{p_Id=skillId},commandType:CommandType.StoredProcedure);
        if(skill is null)
        {
            logger.LogWarning($"user {currentUser.UserId} tries to add an non existing skill to the job {jobId}");
            throw new NotFoundException("Skill Not Found");
        }
        if(job.RecruiterId!=currentUser.UserId && currentUser.Role!=UserRole.Admin)
        {
            logger.LogWarning($"Forbidden:user {currentUser.UserId} tries to add skills to job without proper permission");
            throw new ForbiddenException("You do not have access to this job");
        }
        var query="select * from jobskills where jobId=@p_JobId and skillid=@p_SkillId";
        var exist=await connection.QueryFirstOrDefaultAsync<JobSkills?>(query,parameters);
        if(exist is not null)
        {
            logger.LogWarning($"user {currentUser.UserId} tries to add an already existing skill to job {jobId}");
            throw new ConflictException("Skill already added");
        }
        int rowsaffected=await connection.ExecuteAsync("AddJobSkill",parameters,commandType:CommandType.StoredProcedure);
        if (rowsaffected == 0)
        {
            logger.LogCritical("Adding skill failed:Database responded with 0 rowws affected");
            throw new Exception("Internal server Exception");
        }
        logger.LogInformation($"Added skill {skillId} to job {jobId} successfully");
        return true;
    }

    public async Task<IEnumerable<SkillDto>> GetJobSkills(int jobId)
    {
        using var connection=context.GetConnection();
        Job? job=await connection.QueryFirstOrDefaultAsync<Job?>("GetJobById",new {p_Id=jobId},commandType:CommandType.StoredProcedure);
        if(job is null)
        {
            if(currentUser.IsAuthenticated) logger.LogWarning($"User {currentUser.UserId} tried to access skills of non-existing job");
            else logger.LogWarning("Someone tried to access skills of non-existing job");
            throw new NotFoundException("Job Not Found");
        }
        var skills=await connection.QueryAsync<Skills>("GetJobSkills",new{p_JobId=jobId},commandType:CommandType.StoredProcedure);
        return skills.Select(x=>x.ToDto());
    }

    public async Task<bool> RemoveSkillFromJob(int jobId, int skillId)
    {
        if (!currentUser.IsAuthenticated)
        {
            logger.LogWarning("Someone tried to remove skill from the job without proper authentication");
            throw new UnauthorizedException("Unauthorized");
        }
        if(currentUser.Role!=UserRole.Admin && currentUser.Role!=UserRole.Recruiter){
            logger.LogWarning($"Forbidden:user {currentUser.UserId} tries to remove skills from job without proper permission");
            throw new ForbiddenException("You don't have access to perform this operation");
        }
        using var connection=context.GetConnection();
        var parameters=new {p_JobId=jobId,p_SkillId=skillId};
        Job? job=await connection.QueryFirstOrDefaultAsync<Job?>("GetJobById",new {p_Id=jobId},commandType:CommandType.StoredProcedure);
        if(job is null)
        {
            logger.LogWarning($"user {currentUser.UserId} tried to remove skills to non-existing job");
            throw new NotFoundException("Job Not Found");
        }
        Skills? skill=await connection.QueryFirstOrDefaultAsync<Skills?>("GetSkillById",new{p_Id=skillId},commandType:CommandType.StoredProcedure);
        if(skill is null)
        {
            logger.LogWarning($"user {currentUser.UserId} tries to remove an non existing skill from the job {jobId}");
            throw new NotFoundException("Skill Not Found");
        }
        if(job.RecruiterId!=currentUser.UserId && currentUser.Role!=UserRole.Admin)
        {
            logger.LogWarning($"Forbidden:user {currentUser.UserId} tries to remove skills from the job {jobId} without proper permission");
            throw new ForbiddenException("You do not have access to this job");
        }
        string query="select * from jobskills where jobId=@p_JobId and skillid=@p_SkillId";
        var exist=await connection.QueryFirstOrDefaultAsync<JobSkills?>(query,parameters);
        if(exist is null)
        {
            logger.LogWarning($"user {currentUser.UserId} tries to remove a skill that is not attached to tht job");
            throw new ConflictException("skill was not associated with this job");
        }
        int rowsaffected=await connection.ExecuteAsync("RemoveJobSkill",parameters,commandType:CommandType.StoredProcedure);
        if (rowsaffected == 0)
        {
            logger.LogCritical("Removing skill failed:Database responded with 0 rowws affected");
            throw new Exception("Internal server Exception");
        }
        logger.LogInformation($"Removed the skill {skillId} from job {jobId} successfully");
        return true;
    }
}