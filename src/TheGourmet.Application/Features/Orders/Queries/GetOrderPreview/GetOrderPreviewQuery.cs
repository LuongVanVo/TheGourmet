using MediatR;
using TheGourmet.Application.DTOs.Order;

namespace TheGourmet.Application.Features.Orders.Queries.GetOrderPreview;

public class GetOrderPreviewQuery : IRequest<OrderPreviewDto>
{
    public List<CreateOrderItemDto> OrderItems { get; set; }
    public string? VoucherCode { get; set; }
}