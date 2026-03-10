using MediatR;
using Microsoft.AspNetCore.Http;
using TheGourmet.Application.DTOs.Payment;

namespace TheGourmet.Application.Features.Payments.Queries.GetVnPayReturnResult;

public class GetVnPayReturnResultQuery : IRequest<PaymentResponseModel>
{
    public IQueryCollection QueryData { get; set; } = null!;
}