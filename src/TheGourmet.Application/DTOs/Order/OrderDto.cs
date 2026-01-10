namespace TheGourmet.Application.DTOs.Order;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedDate { get; set; }
    public decimal TotalAmount => OrderItems.Sum(oi => oi.TotalLineAmount);
    public string Status { get; set; } = string.Empty;
    public string? PaymentTransactionId { get; set; }
    
    public decimal ShippingFee { get; set; }
    
    public decimal TotalOrderAmount => TotalAmount + ShippingFee;
    
    // List of order items
    public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
}
