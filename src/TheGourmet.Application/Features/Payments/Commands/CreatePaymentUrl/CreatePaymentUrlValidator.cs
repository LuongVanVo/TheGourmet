using System.Net;
using FluentValidation;

namespace TheGourmet.Application.Features.Payments.Commands.CreatePaymentUrl;

public class CreatePaymentUrlValidator : AbstractValidator<CreatePaymentUrlCommand>
{
    public CreatePaymentUrlValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID is required.")
            .Must(x => x != Guid.Empty).WithMessage("Order ID must be a valid GUID.");
        
        RuleFor(x => x.IpAddress)
            .NotEmpty().WithMessage("IP address is required.")
            .Must(ip => IPAddress.TryParse(ip, out _)).WithMessage("Invalid IP address format.");
    }
}