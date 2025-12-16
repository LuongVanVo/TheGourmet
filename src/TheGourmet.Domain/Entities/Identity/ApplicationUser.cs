using Microsoft.AspNetCore.Identity;

namespace TheGourmet.Domain.Entities.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public required string Fullname { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
}