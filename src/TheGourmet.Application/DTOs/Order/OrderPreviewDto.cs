namespace TheGourmet.Application.DTOs.Order;

public class OrderPreviewDto
{
    public decimal SubTotal { get; set; } // Money before discounts and taxes
    public decimal ShippingFee { get; set; } // Shipping cost
    public decimal DiscountAmount { get; set; } // Total discount applied
    public decimal TotalAmount { get; set; } // Final amount to be paid
    
    public string? ApplidVoucherCode { get; set; } // Applied voucher code, if any
    public Guid? VoucherId { get; set; }
    public string? Message { get; set; } // Additional message or note
}