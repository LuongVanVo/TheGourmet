using TheGourmet.Application.DTOs.Order;

namespace TheGourmet.Application.Interfaces;

public interface IOrderCalculationService
{
    Task<OrderPreviewDto> CalculateOrderAsync(List<CreateOrderItemDto> items, string? voucherCode);
}