using LearnHubApi.Dtos.Modules;
using LearnHubApi.Entities;

namespace LearnHubApi.Extensions;

public static class ModuleExtension
{
    public static ModuleDto ToDto(this Module module)
    {
        return new ModuleDto(module.Id,module.Title,module.Description,module.Order,module.CourseId,module.Course.Title,module.Lessons.Count);
    }
}