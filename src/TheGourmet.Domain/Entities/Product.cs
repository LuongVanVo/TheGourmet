using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TheGourmet.Domain.Common;

namespace TheGourmet.Domain.Entities;

public class Product : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? OriginalPrice { get; set; } // Giá gốc (để gạch chéo nếu có khuyến mãi)

    public int StockQuantity { get; set; } // Số lượng tồn kho
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public double AverageRating { get; set; } = 0.0;
    public int ReviewCount { get; set; } = 0;
    
    // Verion for concurrency control
    [Timestamp]
    public uint RowVersion { get; set; }
    
    // Relationships
    public Guid CategoryId { get; set; }
    
    // virtual for lazy loading
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<ProductReview> Reviews { get; set; } = null!;
}