using MimeKit;
using TheGourmet.Application.Common;
using TheGourmet.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace TheGourmet.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;
        public EmailService(MailSettings mailSettings)
        {
            _mailSettings = mailSettings;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            // 1. Tạo nội dung email
            var email = new MimeMessage();

            // From (người gửi)
            email.From.Add(new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail));

            // To (người nhận)
            email.To.Add(MailboxAddress.Parse(toEmail));

            // Subject (tiêu đề)
            email.Subject = subject;

            // Body (nội dung)
            var builder = new BodyBuilder();
            builder.HtmlBody = htmlMessage;
            email.Body = builder.ToMessageBody();

            // 2. Kết nối đến SMTP server và gửi email
            using var smtp = new SmtpClient();
            try
            {
                // Kết nối đến SMTP server qua Port 587 với StartTLS
                await smtp.ConnectAsync(_mailSettings.Server, _mailSettings.Port, SecureSocketOptions.StartTls);
                // Đăng nhập
                await smtp.AuthenticateAsync(_mailSettings.SenderEmail, _mailSettings.Password);
                // Gửi email
                await smtp.SendAsync(email);
            } catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi email: {ex.Message}");
                throw;
            } finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}