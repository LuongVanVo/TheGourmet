using Microsoft.AspNetCore.Http;

namespace TheGourmet.Application.Interfaces;

public interface ICloudinaryService
{
    Task<string?> UploadImageAsync(IFormFile file, string folderName);
}