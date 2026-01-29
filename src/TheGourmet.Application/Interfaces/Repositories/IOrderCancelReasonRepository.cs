using TheGourmet.Application.DTOs.OrderCancelReason;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Application.Interfaces.Repositories;

public interface IOrderCancelReasonRepository
{
    Task<OrderCancelReason?> GetByIdAsync(Guid id);
    Task<List<OrderCancelReason>> GetAllAsync();
}