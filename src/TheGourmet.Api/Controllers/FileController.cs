using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheGourmet.Application.Interfaces;

namespace TheGourmet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FileController(ICloudinaryService cloudinaryService) : ControllerBase
{
    private readonly ICloudinaryService _cloudinaryService = cloudinaryService;
    
    [Authorize]
    [HttpPost("avatar")]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        var url = await _cloudinaryService.UploadImageAsync(file, "avatars");
        return Ok(new { url });
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost("product-image")]
    public async Task<IActionResult> UploadProductImage(IFormFile file)
    {
        var url = await _cloudinaryService.UploadImageAsync(file, "products");
        return Ok(new { url });
    }
    
    // General file upload
    [Authorize]
    [HttpPost("upload/{type}")]
    public async Task<IActionResult> UploadGeneral(IFormFile file, string type)
    {
        var allowedTypes = new[] { "avatars", "products", "others" };
        if (!allowedTypes.Contains(type.ToLower())) 
        {
            return BadRequest("Invalid upload type.");
        }
        
        var url = await _cloudinaryService.UploadImageAsync(file, type.ToLower());
        return Ok(new { url });
    }
}