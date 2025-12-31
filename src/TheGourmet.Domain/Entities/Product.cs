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
    
    // Relationships
    public Guid CategoryId { get; set; }
    
    // virtual for lazy loading
    public virtual Category Category { get; set; } = null!;
}