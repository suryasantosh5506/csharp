using LearnHubApi.Dtos.Category;
using LearnHubApi.Entities;

namespace LearnHubApi.Extensions;

public static class CategoryExtension
{
    public static CategoryDto ToDto(this Category category)
    {
        return new CategoryDto(category.Id,category.Name,category.Description);
    }
}