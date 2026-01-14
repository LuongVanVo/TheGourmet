using Hangfire;
using MassTransit;
using MediatR;
using TheGourmet.Application.Common.Events;
using TheGourmet.Application.Common.ExternalServices;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Orders.Results;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Entities;
using TheGourmet.Domain.Enums;

namespace TheGourmet.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    public CreateOrderHandler(IUnitOfWork unitOfWork, IUserRepository userRepository, IPublishEndpoint publishEndpoint)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<OrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                CreatedDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                PaymentExpiredAt = DateTime.UtcNow.AddDays(1),
                OrderItems = new List<OrderItem>(),
                TotalAmount = 0,
                ReceiverName = request.ReceiverName,
                ReceiverPhone = request.ReceiverPhone,
                ShippingAddress = request.ShippingAddress,
                Note = request.Note,
            };
            decimal subTotal = 0;

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
                subTotal += orderItem.UnitPrice * orderItem.Quantity;
                order.OrderItems.Add(orderItem);
            }

            decimal shippingFee = 0;

            if (subTotal > 500000)
            {
                shippingFee = subTotal * 0.02m;
            }
            else
            {
                shippingFee = subTotal * 0.05m;
            }
            
            order.ShippingFee = shippingFee;
            order.TotalAmount = subTotal + shippingFee;
            
            await _unitOfWork.Orders.AddOrderAsync(order);
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
        foreach (var item in order.OrderItems)
        {
            itemsHtml += $@"
                <tr>
                    <td style='padding: 8px; border-bottom: 1px solid #ddd;'>{item.ProductName}</td>
                    <td style='padding: 8px; border-bottom: 1px solid #ddd; text-align: center;'>{item.Quantity}</td>
                    <td style='padding: 8px; border-bottom: 1px solid #ddd; text-align: right;'>{item.UnitPrice:N0} đ</td>
                    <td style='padding: 8px; border-bottom: 1px solid #ddd; text-align: right;'>{(item.UnitPrice * item.Quantity):N0} đ</td>
                </tr>";
        }

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
                                    <td colspan='3' style='text-align: right; padding: 8px; font-weight: bold;'>Phí vận chuyển:</td>
                                    <td style='text-align: right; padding: 8px;'>{order.ShippingFee:N0} đ</td>
                                </tr>
                                <tr>
                                    <td colspan='3' style='text-align: right; padding: 8px; font-weight: bold; color: #d9534f; font-size: 1.1em;'>Tổng cộng:</td>
                                    <td style='text-align: right; padding: 8px; font-weight: bold; color: #d9534f; font-size: 1.1em;'>{order.TotalAmount:N0} đ</td>
                                </tr>
                            </tfoot>
                        </table>

                        <div style='margin-top: 20px; background-color: #f9f9f9; padding: 10px;'>
                            <p><strong>Người nhận:</strong> {order.ReceiverName} ({order.ReceiverPhone})</p>
                            <p><strong>Địa chỉ giao:</strong> {order.ShippingAddress}</p>
                        </div>

                        <p style='margin-top: 20px; font-size: 0.9em; color: #777;'>
                            Nếu bạn cần hỗ trợ, vui lòng liên hệ hotline hoặc trả lời email này.<br>
                            Trân trọng,<br>
                            <strong>The Gourmet Team</strong>
                        </p>
                    </div>
                </body>
            </html>";
    }
}