using MediatR;
using TheGourmet.Application.DTOs.Payment;

namespace TheGourmet.Application.Features.Payments.Queries.GetAllPayments;

public class GetAllPaymentsQuery : IRequest<IEnumerable<PaymentTransactionDto>>
{
    
}