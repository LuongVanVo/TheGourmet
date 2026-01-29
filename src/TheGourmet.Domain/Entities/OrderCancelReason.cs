using TheGourmet.Domain.Common;

namespace TheGourmet.Domain.Entities;

public class OrderCancelReason : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation
    public virtual ICollection<Order> Orders { get; set; }
}