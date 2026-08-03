using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LearnHubApi.Interfaces;

namespace  LearnHubApi.Services;

public class CloudinaryService:ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IConfiguration configuration)
    {
        var CloudName=configuration["Cloudinary:CloudName"];
        var ApiKey=configuration["Cloudinary:ApiKey"];
        var ApiSecret=configuration["Cloudinary:ApiSecret"];
        
        var acc=new Account(CloudName,ApiKey,ApiSecret);
        _cloudinary=new Cloudinary(acc);
    }

    public async Task<VideoUploadResult> VideoUploadAsync(IFormFile file)
    {
        var result=new VideoUploadResult();
        if (file is null || file.Length==0) throw new Exception("No video selected.");
        
        using var stream=file.OpenReadStream();
        var uploadParams=new VideoUploadParams
        {
            File=new FileDescription(file.FileName,stream),
            Folder="LMS.DotNet"
        };
        result=await _cloudinary.UploadAsync(uploadParams);
        
        return result;
    }

    public async Task<DeletionResult> DeleteVideoAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Video
        };

        var result=await _cloudinary.DestroyAsync(deleteParams);
        return result;
    }

    public async Task<ImageUploadResult> ImageUploadAsync(IFormFile file)
    {
        var result=new ImageUploadResult();
        if (file is null || file.Length==0) throw new Exception("No Image selected.");
        using var stream=file.OpenReadStream();

        var uploadParams=new ImageUploadParams
        {
            File=new FileDescription(file.FileName,stream),
            Folder="thumbnails"
        };

        result=await _cloudinary.UploadAsync(uploadParams);
        return result;
    }

    public async Task<DeletionResult> DeleteImageAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Image
        };

        var result=await _cloudinary.DestroyAsync(deleteParams);
        return result;
    }
}