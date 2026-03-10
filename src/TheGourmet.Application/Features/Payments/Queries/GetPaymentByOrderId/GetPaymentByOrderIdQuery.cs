using MediatR;
using TheGourmet.Application.DTOs.Payment;

namespace TheGourmet.Application.Features.Payments.Queries.GetPaymentByOrderId;

public class GetPaymentByOrderIdQuery : IRequest<List<PaymentTransactionDto>>
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
}