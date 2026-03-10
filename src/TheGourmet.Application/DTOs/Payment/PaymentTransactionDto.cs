namespace TheGourmet.Application.DTOs.Payment;

public class PaymentTransactionDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string BankCode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}