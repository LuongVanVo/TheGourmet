using System.ComponentModel.DataAnnotations.Schema;
using TheGourmet.Domain.Enums;

namespace TheGourmet.Domain.Entities;

public class Order
{
    public Order()
    {
        // Initialize list to avoid NullReferenceException
        OrderItems = new List<OrderItem>();
        CreatedDate = DateTime.UtcNow;
        Status = OrderStatus.Pending;
        
        // Default order will expire after 1 day if not yet paid
        PaymentExpiredAt = DateTime.UtcNow.AddDays(1);
    }
    
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime PaymentExpiredAt { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal ShippingFee { get; set; }
    public decimal? DiscountAmount { get; set; } = 0;
    public Guid? VoucherId { get; set; }
    public OrderStatus Status { get; set; }
    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverPhone { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string? ReasonCancel { get; set; } // Reason for cancellation, if any
    
    // Code transaction from payment gateway (VNPay, MoMo, etc)
    public string? PaymentTransactionsId { get; set; }
    
    // Relationships
    public virtual ICollection<OrderItem> OrderItems { get; set; }
    [ForeignKey("VoucherId")]
    public virtual Voucher? Voucher { get; set; }
}