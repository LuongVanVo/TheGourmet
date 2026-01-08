namespace TheGourmet.Application.DTOs.Cart;

public class CartItemDto
{
    public Guid ProductId { get; set;}
    public string ProductName { get; set; } = string.Empty;
    public string ProductImage { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    
    public int StockRemaining { get; set; }
    
    // Compute price of this cart item
    public decimal SubTotal => Price * Quantity;
}