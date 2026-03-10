using MediatR;
using TheGourmet.Application.Interfaces;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Enums;

namespace TheGourmet.Application.Features.Payments.Commands.ProcessVnPayIpn;

public class ProcessVnPayIpnHandler : IRequestHandler<ProcessVnPayIpnCommand, string>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVNPayService _vnPayService;
    public ProcessVnPayIpnHandler(IUnitOfWork unitOfWork, IVNPayService vnPayService)
    {
        _unitOfWork = unitOfWork;
        _vnPayService = vnPayService;
    }

    public async Task<string> Handle(ProcessVnPayIpnCommand request, CancellationToken cancellationToken)
    {
        // Check signature (Checksum) to verify that Server VNPay call request ?
        var paymentResult = _vnPayService.PaymentExecute(request.QueryData);

        if (paymentResult == null || string.IsNullOrEmpty(paymentResult.Token))
        {
            return "{\"RspCode\":\"97\",\"Message\":\"Invalid signature\"}"; // Mã 97: Chữ ký không hợp lệ
        }

        // Get transaction id
        var isGuid = Guid.TryParse(paymentResult.OrderId, out Guid transactionId);
        if (!isGuid)
            return "{\"RspCode\":\"01\",\"Message\":\"Order not found\"}";

        // Find transaction in DB
        var transaction = await _unitOfWork.PaymentTransactions.GetByIdAsync(transactionId, cancellationToken);
        if (transaction == null)
            return "{\"RspCode\":\"01\",\"Message\":\"Order not found\"}";

        // Check amount from VNPay with amount in DB
        if (transaction.Amount != (decimal)(int.Parse(request.QueryData["vnp_Amount"]) / 100))
            return "{\"RspCode\":\"04\",\"Message\":\"Invalid amount\"}";

        // Check Idempotency
        if (transaction.Status == "Success" || transaction.Status == "Failed")
            return "{\"RspCode\":\"02\",\"Message\":\"Order already confirmed\"}";

        // Update status
        transaction.ProviderTransactionId = paymentResult.TransactionId;
        transaction.ResponseCode = paymentResult.VnPayResponseCode;

        var order = await _unitOfWork.Orders.GetByIdAsync(transaction.OrderId);

        if (paymentResult.Success)
        {
            // Transaction successful
            transaction.Status = "Success";
            transaction.Message = "Payment successful";

            if (order != null)
            {
                order.Status = OrderStatus.Paid;
                await _unitOfWork.Orders.UpdateOrderAsync(order);
            }
        }
        else
        {
            // Transaction failed
            transaction.Status = "Failed";
            transaction.Message = $"Payment failed. Error: {paymentResult.VnPayResponseCode}";
        }

        // Update transaction in DB
        _unitOfWork.PaymentTransactions.Update(transaction);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return "{\"RspCode\":\"00\",\"Message\":\"Confirm Success\"}";
    }
}