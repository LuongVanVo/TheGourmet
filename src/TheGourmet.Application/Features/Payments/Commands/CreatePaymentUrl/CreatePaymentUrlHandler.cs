using AutoMapper;
using MediatR;
using TheGourmet.Application.DTOs.Payment;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Interfaces;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Entities;
using TheGourmet.Domain.Enums;

namespace TheGourmet.Application.Features.Payments.Commands.CreatePaymentUrl;

public class CreatePaymentUrlHandler : IRequestHandler<CreatePaymentUrlCommand, string>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVNPayService _vnPayService;
    private readonly IMapper _mapper;
    public CreatePaymentUrlHandler(IUnitOfWork unitOfWork, IVNPayService vnPayService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _vnPayService = vnPayService;
        _mapper = mapper;
    }

    public async Task<string> Handle(CreatePaymentUrlCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
        if (order == null)
            throw new BadRequestException("Order not found.");

        if (order.UserId != request.UserId)
            throw new BadRequestException("You are not authorized to pay for this order.");

        if (order.Status != OrderStatus.Pending)
            throw new BadRequestException("This order is not in pending payment status");
        
        // Create transaction temp
        var transactionId = Guid.NewGuid();
        var paymentTransaction = new PaymentTransaction
        {
            Id = transactionId,
            OrderId = order.Id,
            Amount = order.TotalAmount,
            PaymentMethod = "VNPay",
            CreatedAt = DateTime.UtcNow,
            ReferenceId = $"{order.Id}_{DateTime.UtcNow.Ticks}"
        };

        await _unitOfWork.PaymentTransactions.AddAsync(paymentTransaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var paymentModel = new PaymentInformationModel
        {
            TransactionId = transactionId.ToString(),
            OrderId = paymentTransaction.OrderId,
            Amount = paymentTransaction.Amount,
            OrderType = "billpayment",
            OrderDescription = $"Payment for order {order.Id}",
            Name = "TheGourmet Order"
        };

        // // Assign transaction id to order
        // paymentModel.TransactionId = transactionId.ToString();

        var paymentUrl = _vnPayService.CreatePaymentUrl(paymentModel, request.IpAddress);

        return paymentUrl;
    }
}