using AutoMapper;
using MediatR;
using TheGourmet.Application.DTOs.Order;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Orders.Results;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Entities;
using TheGourmet.Domain.Enums;

namespace TheGourmet.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    public CreateOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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

            return new OrderResponse
            {
                Id = order.Id,
                Success = true,
                Message = "Order created successfully",
            };
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}