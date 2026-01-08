namespace TheGourmet.Application.DTOs.Cart;

public class CartDto
{
    public Guid UserId { get; set;  }
    public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    
    // Total price of all items in the cart
    public decimal TotalPrice => Items.Sum(i => i.SubTotal);
}