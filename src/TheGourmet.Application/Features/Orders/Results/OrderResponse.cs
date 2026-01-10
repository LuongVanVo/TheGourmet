namespace TheGourmet.Application.Features.Orders.Results;

public class OrderResponse
{
    public Guid Id { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}