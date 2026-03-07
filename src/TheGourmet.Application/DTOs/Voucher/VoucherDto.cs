using TheGourmet.Domain.Enums;

namespace TheGourmet.Application.DTOs.Voucher;

public class VoucherDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    
    public decimal MinOrderAmount { get; set; } // Minimum order amount to apply the voucher
    public decimal? MaxDiscountAmount { get; set; } // If discount type is percentage, this defines the maximum discount amount (VD: 10% up to 50k)
    
    public int Quantity { get; set; } // Total quantity available
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public bool IsActive { get; set; }

    public bool IsExpired => DateTime.UtcNow > EndDate;
}