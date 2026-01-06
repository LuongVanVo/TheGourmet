using TheGourmet.Domain.Common;
namespace TheGourmet.Domain.Entities;

public class Category : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsDeleted { get; set; } = false;
    public Guid? ParentId { get; set; } // Self-referencing foreign key
    
    // Navigation property
    // A category can have multiple products (1 - n)
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; } = new List<Category>();
}