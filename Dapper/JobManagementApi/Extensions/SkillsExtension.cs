using JobManagementApi.Dtos.Skills;
using JobManagementApi.Entities;

namespace JobManagementApi.Extensions;

public static class SkillsExtension
{
    public static SkillDto ToDto(this Skills skill)
    {
        return new(skill.Id,skill.Name);
    }
}