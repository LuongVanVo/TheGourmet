using MassTransit;
using TheGourmet.Application.Common.Events;
using TheGourmet.Application.Interfaces;

namespace TheGourmet.Infrastructure.Consumer
{
    public class ForgotPasswordConsumer : IConsumer<ForgotPasswordEvent>
    {
        private readonly IEmailService _emailService;
        public ForgotPasswordConsumer(IEmailService emailService)
        {
            _emailService = emailService;
        }
        public async Task Consume(ConsumeContext<ForgotPasswordEvent> context)
        {
            var message = context.Message;

            var emailBody = $@"
                <h2>Yêu cầu đặt lại mật khẩu</h2>
                <p>Bấm vào nút dưới đây để đặt lại mật khẩu mới:</p>
                <a href='{message.ResetLink}' style='background: blue; color: white; padding: 10px;'>Đặt lại mật khẩu</a>
                <p>Nếu bạn không yêu cầu, vui lòng bỏ qua email này.</p>";

            await _emailService.SendEmailAsync(message.Email, "Quên mật khẩu", emailBody);
        }
    }
}