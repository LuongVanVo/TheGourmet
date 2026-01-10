using System.ComponentModel.DataAnnotations;

namespace TheGourmet.Application.DTOs.Order;

public class CreateOrderItemDto
{
    [Required]
    public Guid ProductId { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
    public int Quantity { get; set; }
}