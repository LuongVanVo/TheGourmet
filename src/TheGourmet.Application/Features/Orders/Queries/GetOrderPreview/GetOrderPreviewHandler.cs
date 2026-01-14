using MediatR;
using TheGourmet.Application.DTOs.Order;
using TheGourmet.Application.Interfaces;

namespace TheGourmet.Application.Features.Orders.Queries.GetOrderPreview;

public class GetOrderPreviewHandler : IRequestHandler<GetOrderPreviewQuery, OrderPreviewDto>
{
    private readonly IOrderCalculationService _orderCalculationService;
    public GetOrderPreviewHandler(IOrderCalculationService orderCalculationService)
    {
        _orderCalculationService = orderCalculationService;
    }

    public async Task<OrderPreviewDto> Handle(GetOrderPreviewQuery request, CancellationToken cancellationToken)
    {
        return await _orderCalculationService.CalculateOrderAsync(request.OrderItems, request.VoucherCode);
    }
}