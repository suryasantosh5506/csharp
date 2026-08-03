using CloudinaryDotNet.Actions;
using LearnHubApi.Dtos.Category;

namespace LearnHubApi.Interfaces;

public interface ICloudinaryService
{
    Task<VideoUploadResult> VideoUploadAsync(IFormFile file);

    Task<DeletionResult> DeleteVideoAsync(string publicId);

    Task<ImageUploadResult> ImageUploadAsync(IFormFile file);

    Task<DeletionResult> DeleteImageAsync(string publicId);
}
