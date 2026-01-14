using TheGourmet.Domain.Common;
using TheGourmet.Domain.Enums;

namespace TheGourmet.Domain.Entities;

public class Voucher : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    
    public decimal? MaxDiscountAmount { get; set; } // If discount type is percentage, this defines the maximum discount amount (VD: 10% up to 50k)
    public decimal MinOrderAmount { get; set; } // Minimum order amount to apply the voucher
    
    public int Quantity { get; set; } // Total quantity available
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; } = true;
    
    // Relationships
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>(); // 1 voucher can be used in many orders
}