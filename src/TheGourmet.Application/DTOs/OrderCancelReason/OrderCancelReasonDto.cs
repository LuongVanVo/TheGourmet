namespace TheGourmet.Application.DTOs.OrderCancelReason;

public class OrderCancelReasonDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}