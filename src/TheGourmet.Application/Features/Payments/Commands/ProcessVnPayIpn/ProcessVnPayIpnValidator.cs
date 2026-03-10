using FluentValidation;

namespace TheGourmet.Application.Features.Payments.Commands.ProcessVnPayIpn;

public class ProcessVnPayIpnValidator : AbstractValidator<ProcessVnPayIpnCommand>
{
    public ProcessVnPayIpnValidator()
    {
        RuleFor(x => x.QueryData)
            .NotEmpty().WithMessage("Query data is required.")
            .Must(q => q.ContainsKey("vnp_TxnRef") && q.ContainsKey("vnp_ResponseCode") && q.ContainsKey("vnp_SecureHash"))
            .WithMessage("Query data must contain vnp_TxnRef, vnp_ResponseCode, and vnp_SecureHash.");
    }
}