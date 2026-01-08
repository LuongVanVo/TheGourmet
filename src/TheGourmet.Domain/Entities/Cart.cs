using TheGourmet.Domain.Common;

namespace TheGourmet.Domain.Entities;

public class Cart : BaseEntity
{
    public Guid UserId { get; set; }
    
    // Relationships 1-n
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}