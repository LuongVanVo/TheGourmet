using Hangfire;
using MassTransit;
using MediatR;
using TheGourmet.Application.Common.Events;
using TheGourmet.Application.Common.ExternalServices;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Orders.Results;
using TheGourmet.Application.Interfaces;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Entities;
using TheGourmet.Domain.Enums;

namespace TheGourmet.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IOrderCalculationService _orderCalculationService;
    public CreateOrderHandler(IUnitOfWork unitOfWork, IUserRepository userRepository, IPublishEndpoint publishEndpoint, IOrderCalculationService orderCalculationService)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _publishEndpoint = publishEndpoint;
        _orderCalculationService = orderCalculationService;
    }

    public async Task<OrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var calculationResult =
                await _orderCalculationService.CalculateOrderAsync(request.OrderItems, request.VoucherCode);
            
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                CreatedDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                PaymentExpiredAt = DateTime.UtcNow.AddDays(1),
                OrderItems = new List<OrderItem>(),
                TotalAmount = calculationResult.TotalAmount,
                ShippingFee = calculationResult.ShippingFee,
                DiscountAmount = calculationResult.DiscountAmount,
                VoucherId = calculationResult.VoucherId,
                ReceiverName = request.ReceiverName,
                ReceiverPhone = request.ReceiverPhone,
                ShippingAddress = request.ShippingAddress,
                Note = request.Note,
            };

            foreach (var itemInput in request.OrderItems)
            {
                bool isReserved = await _unitOfWork.Products.DecreaseStockAtomicAsync(itemInput.ProductId, itemInput.Quantity);
                if (!isReserved)
                    throw new BadRequestException($"Product with ID {itemInput.ProductId} does not have enough stock.");

                var product = await _unitOfWork.Products.GetProductByIdAsync(itemInput.ProductId);
                if (product == null) throw new BadRequestException("Product not exist.");

                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = itemInput.Quantity,
                };
                order.OrderItems.Add(orderItem);
            }
            
            await _unitOfWork.Orders.AddOrderAsync(order);

            if (calculationResult.VoucherId.HasValue)
            {
                await _unitOfWork.Vouchers.DecreaseQuantityAsync(calculationResult.VoucherId.Value);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync();
            
            // Hangfire 
            var jobId = BackgroundJob.Schedule<OrderTimeoutJob>(
                job => job.CheckAndCancelOrderAsync(order.Id),
                TimeSpan.FromDays(1));
            
            try
            {
                // send email notification 
                var userEmail = await _userRepository.GetEmailByUserIdAsync(request.UserId);

                var emailSubject = $"Xác nhận đơn hàng #{order.Id} - The Gourmet";
                var emailBody = GenerateOrderEmailBody(order, request.ReceiverName);

                await _publishEndpoint.Publish(new OrderCreatedEvent
                {
                    Email = userEmail,
                    Subject = emailSubject,
                    Body = emailBody
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to publish OrderCreatedEvent: " + ex.Message);
            }
            
            return new OrderResponse
            {
                Id = order.Id,
                Success = true,
                Message = "Order created successfully. Please complete the payment within 24 hours.",
            };
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
    
    private string GenerateOrderEmailBody(Order order, string customerName)
    {
        var itemsHtml = "";
        decimal subTotal = 0;

        // Calculate subtotal and generate items rows
        foreach (var item in order.OrderItems)
        {
            var itemTotal = item.UnitPrice * item.Quantity;
            subTotal += itemTotal;

            itemsHtml += $@"
                <tr>
                    <td style='padding: 8px; border-bottom: 1px solid #ddd;'>{item.ProductName}</td>
                    <td style='padding: 8px; border-bottom: 1px solid #ddd; text-align: center;'>{item.Quantity}</td>
                    <td style='padding: 8px; border-bottom: 1px solid #ddd; text-align: right;'>{item.UnitPrice:N0} đ</td>
                    <td style='padding: 8px; border-bottom: 1px solid #ddd; text-align: right;'>{itemTotal:N0} đ</td>
                </tr>";
        }

        // Add discount row if applicable
        var discountRowHtml = "";
        if (order.DiscountAmount > 0)
        {
            discountRowHtml = $@"
                <tr>
                    <td colspan='3' style='text-align: right; padding: 8px; font-weight: bold; color: #28a745;'>Giảm giá (Voucher):</td>
                    <td style='text-align: right; padding: 8px; color: #28a745;'>-{order.DiscountAmount:N0} đ</td>
                </tr>";
        }

        // HTML Email Body
        return $@"
            <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; border: 1px solid #eee; padding: 20px;'>
                        <h2 style='color: #4CAF50;'>Cảm ơn {customerName} đã đặt hàng!</h2>
                        <p>Đơn hàng <strong>#{order.Id}</strong> của bạn đã được tiếp nhận.</p>
                        
                        <h3>Chi tiết đơn hàng</h3>
                        <table style='width: 100%; border-collapse: collapse;'>
                            <thead>
                                <tr style='background-color: #f8f9fa;'>
                                    <th style='text-align: left; padding: 8px;'>Sản phẩm</th>
                                    <th style='text-align: center; padding: 8px;'>SL</th>
                                    <th style='text-align: right; padding: 8px;'>Đơn giá</th>
                                    <th style='text-align: right; padding: 8px;'>Thành tiền</th>
                                </tr>
                            </thead>
                            <tbody>
                                {itemsHtml}
                            </tbody>
                            <tfoot>
                                <tr>
                                    <td colspan='3' style='text-align: right; padding: 8px; font-weight: bold;'>Tạm tính:</td>
                                    <td style='text-align: right; padding: 8px;'>{subTotal:N0} đ</td>
                                </tr>
                                
                                <tr>
                                    <td colspan='3' style='text-align: right; padding: 8px; font-weight: bold;'>Phí vận chuyển:</td>
                                    <td style='text-align: right; padding: 8px;'>{order.ShippingFee:N0} đ</td>
                                </tr>

                                {discountRowHtml}

                                <tr style='border-top: 2px solid #eee;'>
                                    <td colspan='3' style='text-align: right; padding: 8px; font-weight: bold; color: #d9534f; font-size: 1.2em;'>Tổng thanh toán:</td>
                                    <td style='text-align: right; padding: 8px; font-weight: bold; color: #d9534f; font-size: 1.2em;'>{order.TotalAmount:N0} đ</td>
                                </tr>
                            </tfoot>
                        </table>

                        <div style='margin-top: 20px; background-color: #f9f9f9; padding: 15px; border-radius: 5px;'>
                            <h4 style='margin-top: 0;'>Thông tin giao hàng</h4>
                            <p style='margin: 5px 0;'><strong>Người nhận:</strong> {order.ReceiverName} ({order.ReceiverPhone})</p>
                            <p style='margin: 5px 0;'><strong>Địa chỉ:</strong> {order.ShippingAddress}</p>
                            <p style='margin: 5px 0;'><strong>Ghi chú:</strong> {order.Note ?? "Không có"}</p>
                        </div>

                        <p style='margin-top: 20px; font-size: 0.9em; color: #777; text-align: center;'>
                            Nếu bạn cần hỗ trợ, vui lòng trả lời email này.<br>
                            Trân trọng,<br>
                            <strong>The Gourmet Team</strong>
                        </p>
                    </div>
                </body>
            </html>";
    }
}