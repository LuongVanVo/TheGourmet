using MassTransit;
using Microsoft.Extensions.Logging;
using TheGourmet.Application.Common.Events;
using TheGourmet.Application.Interfaces;

namespace TheGourmet.Infrastructure.Consumer;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<OrderCreatedConsumer> _logger;
    public OrderCreatedConsumer(IEmailService emailService, ILogger<OrderCreatedConsumer> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation($"Sending order confirmation email to {message.Email}");

        try
        {
            await _emailService.SendEmailAsync(message.Email?.ToString(), message.Subject, message.Body);
            
            _logger.LogInformation("Send email successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send email.");
            throw;
        }
    }
}