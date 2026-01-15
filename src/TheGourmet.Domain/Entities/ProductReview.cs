using TheGourmet.Domain.Common;
using TheGourmet.Domain.Entities.Identity;

namespace TheGourmet.Domain.Entities;

public class ProductReview : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public Guid OrderId { get; set; }
    
    public int Rating { get; set; } // e.g., 1 to 5
    public string? Comments { get; set; }
    public string? Reply { get; set; }
    
    // Navigation properties
    public virtual Product Product { get; set; } = null!;
    public virtual ApplicationUser? User { get; set; } = null!;
}