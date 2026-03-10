namespace TheGourmet.Application.DTOs.Payment;

public class PaymentInformationModel
{
    public string OrderType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string OrderDescription { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
}