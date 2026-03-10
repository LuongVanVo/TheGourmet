using TheGourmet.Domain.Common;

namespace TheGourmet.Domain.Entities;

public class PaymentTransaction : BaseEntity
{
    public Guid OrderId { get; set; }
    public string ReferenceId { get; set; } = string.Empty; // Mã giao dịch - vnp_TxnRef
    public string? ProviderTransactionId { get; set; } // vnp_TransactionNo
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty; // VNPay, MOMO ...
    public string Status { get; set; } = string.Empty; // Success, Failed, Pending
    public string? ResponseCode { get; set; } // vnp_ResponseCode
    public string? Message { get; set; } // vnp_Message
    public string? RawPayload { get; set; } // Lưu trữ toàn bộ payload trả về từ cổng thanh toán (dành cho mục đích debug)
    
    public virtual Order Order { get; set; }
}