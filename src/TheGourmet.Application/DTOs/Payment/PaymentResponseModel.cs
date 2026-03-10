namespace TheGourmet.Application.DTOs.Payment;

public class PaymentResponseModel
{
    public string OrderDescription { get; set; } = string.Empty; // Nội dung thanh toán VNPay trả về
    public string TransactionId { get; set; } = string.Empty; // Mã giao dịch VNPay trả về
    public string OrderId { get; set; } // Mã đơn hàng của hệ thống
    public string PaymentMethod { get; set; } = string.Empty; // Phương thức thanh toán (VD: VNPay, COD)
    public string PaymentId { get; set; } = string.Empty;
    public bool Success { get; set; } // Trạng thái thanh toán
    public string Token { get; set; } = string.Empty;
    public string VnPayResponseCode { get; set; } = string.Empty;
}