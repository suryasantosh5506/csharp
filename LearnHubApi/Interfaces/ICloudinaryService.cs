using CloudinaryDotNet.Actions;
using LearnHubApi.Dtos.Category;

namespace LearnHubApi.Interfaces;

public interface ICloudinaryService
{
    public Task<VideoUploadResult> VideoUploadAsync(IFormFile file);

    public Task<DeletionResult> DeleteVideoAsync(string publicId);
}
