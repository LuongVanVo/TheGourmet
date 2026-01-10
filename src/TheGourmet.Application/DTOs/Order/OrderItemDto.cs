namespace TheGourmet.Application.DTOs.Order;

public class OrderItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    
    // Total price for this item (UnitPrice * Quantity)
    public decimal TotalLineAmount { get; set; }
}