using AutoMapper;
using MediatR;
using TheGourmet.Application.DTOs.Payment;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Payments.Queries.GetPaymentByOrderId;

public class GetPaymentByOrderIdHandler : IRequestHandler<GetPaymentByOrderIdQuery, List<PaymentTransactionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public GetPaymentByOrderIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<PaymentTransactionDto>> Handle(GetPaymentByOrderIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
        if (order == null) throw new NotFoundException("Order not found.");

        if (order.UserId != request.UserId)
            throw new BadRequestException("You are not authorized to view payment information for this order.");

        var payments = await _unitOfWork.PaymentTransactions.GetByOrderIdAsync(request.OrderId, cancellationToken);
        if (payments == null! || !payments.Any()) return new List<PaymentTransactionDto>();

        return _mapper.Map<List<PaymentTransactionDto>>(payments);
    }
}