using TheGourmet.Domain.Entities;

namespace TheGourmet.Application.Interfaces.Repositories;

public interface IPaymentTransactionRepository
{
    // Dùng để tìm lại Giao dịch khi VNPay gọi IPN trả về vnp_TxnRef
    Task<PaymentTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    // Dùng lúc khách click "Thanh toán" để lưu 1 dòng Pending vào DB
    Task AddAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default);
    
    // Dùng lúc nhận được IPN để cập nhật trạng thái thành Success/Failed
    void Update(PaymentTransaction transaction);
    
    // Dùng cho User xem lịch sử giao dịch của đơn hàng
    Task<IEnumerable<PaymentTransaction>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    
    // Dùng cho Admin đối soát với file Excel của VNPay (tìm theo mã giao dịch vnp_TransactionNo)
    Task<PaymentTransaction?> GetByProviderTransactionIdAsync(string providerTransactionId, CancellationToken cancellationToken = default);

    // Lấy tất cả giao dịch cho Admin
    Task<IEnumerable<PaymentTransaction>> GetAllAsync(CancellationToken cancellationToken = default);
}