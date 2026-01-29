using MediatR;
using TheGourmet.Application.DTOs.OrderCancelReason;

namespace TheGourmet.Application.Features.Orders.Queries.GetCancelReasons;

public class GetCancelReasonsQuery : IRequest<List<OrderCancelReasonDto>>
{
    
}