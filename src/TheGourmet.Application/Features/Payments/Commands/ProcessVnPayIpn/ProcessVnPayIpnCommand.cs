using MediatR;
using Microsoft.AspNetCore.Http;

namespace TheGourmet.Application.Features.Payments.Commands.ProcessVnPayIpn;

public class ProcessVnPayIpnCommand : IRequest<string>
{
    public IQueryCollection QueryData { get; set; }
}