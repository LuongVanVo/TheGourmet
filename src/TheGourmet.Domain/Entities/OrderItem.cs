using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheGourmet.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    
    // Snapshot of name product at the time of order
    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;
    
    // Snapshot of selling price at the time order
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    
    // Navigation property
    public virtual Order Order { get; set; } 
    public virtual Product Product { get; set; }
}