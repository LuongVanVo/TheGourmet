using TheGourmet.Domain.Entities;

namespace TheGourmet.Application.Interfaces.Repositories;

public interface IProductReviewRepository
{
    // Thêm review mới
    Task AddAsync(ProductReview review);
    
    // check user đã review sản phẩm trong đơn hàng chưa
    Task<bool> HasUserReviewedOrderedProductAsync(Guid userId, Guid productId, Guid orderId);

    // get list review by product id 
    Task<List<ProductReview>> GetByProductIdAsync(Guid productId);
    
    // helper to support complex queries
    IQueryable<ProductReview> GetQueryable();
}