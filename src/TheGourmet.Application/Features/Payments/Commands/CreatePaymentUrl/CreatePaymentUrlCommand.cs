using MediatR;

namespace TheGourmet.Application.Features.Payments.Commands.CreatePaymentUrl;

public class CreatePaymentUrlCommand : IRequest<string>
{
    public Guid OrderId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}