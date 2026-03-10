using MediatR;
using TheGourmet.Application.DTOs.Payment;
using TheGourmet.Application.Interfaces;

namespace TheGourmet.Application.Features.Payments.Queries.GetVnPayReturnResult;

public class GetVnPayReturnResultHandler : IRequestHandler<GetVnPayReturnResultQuery, PaymentResponseModel>
{
    private readonly IVNPayService _vnPayService;
    public GetVnPayReturnResultHandler(IVNPayService vnPayService)
    {
        _vnPayService = vnPayService;
    }

    public Task<PaymentResponseModel> Handle(GetVnPayReturnResultQuery request, CancellationToken cancellationToken)
    {
        var paymentResult = _vnPayService.PaymentExecute(request.QueryData);

        return Task.FromResult(paymentResult);
    }
}