using AutoMapper;
using MediatR;
using TheGourmet.Application.DTOs.Payment;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Payments.Queries.GetAllPayments;

public class GetAllPaymentsHandler : IRequestHandler<GetAllPaymentsQuery, IEnumerable<PaymentTransactionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public GetAllPaymentsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PaymentTransactionDto>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
    {
        var payments = await _unitOfWork.PaymentTransactions.GetAllAsync(cancellationToken);

        var paymentDtos = _mapper.Map<IEnumerable<PaymentTransactionDto>>(payments);

        return paymentDtos.OrderByDescending(p => p.CreatedDate);
    }
}