namespace TheGourmet.Application.Features.Vouchers.Results;

public class VoucherResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}