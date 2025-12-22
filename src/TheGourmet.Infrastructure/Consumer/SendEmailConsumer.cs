using MassTransit;
using TheGourmet.Application.Common.Events;
using TheGourmet.Application.Interfaces;

namespace TheGourmet.Infrastructure.Consumer
{
    public class SendEmailConsumer : IConsumer<UserRegisteredEvent>
    {
        private readonly IEmailService _emailService;
        public SendEmailConsumer(IEmailService emailService)
        {
            _emailService = emailService;
        }

        // Hàm này sẽ tự chạy khi có tin nhắn đến
        public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
        {
            var message = context.Message;

            await _emailService.SendEmailAsync(message.Email, message.Subject, message.Body);
        }
    }
}