using TheGourmet.Domain.Common;
namespace TheGourmet.Domain.Entities;

public class Category : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsDeleted { get; set; } = false;
    
    // Navigation property
    // A category can have multiple products (1 - n)
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}