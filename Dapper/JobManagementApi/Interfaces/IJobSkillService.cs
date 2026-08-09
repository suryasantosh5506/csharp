using JobManagementApi.Dtos.Skills;

namespace JobManagementApi.Interfaces;

public interface IJobSkillService
{
    Task<bool> AddSkillToJob(int jobId,int skillId);

    Task<bool> RemoveSkillFromJob(int jobId,int skillId);

    Task<IEnumerable<SkillDto>> GetJobSkills(int jobId);
}