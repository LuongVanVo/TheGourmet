using MimeKit;
using TheGourmet.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using TheGourmet.Application.Common.ExternalSettings;

namespace TheGourmet.Infrastructure.Services
{
    public class EmailService(MailSettings mailSettings) : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            // 1. Tạo nội dung email
            var email = new MimeMessage();

            // From (người gửi)
            email.From.Add(new MailboxAddress(mailSettings.SenderName, mailSettings.SenderEmail));

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
                await smtp.ConnectAsync(mailSettings.Server, mailSettings.Port, SecureSocketOptions.StartTls);
                // Đăng nhập
                await smtp.AuthenticateAsync(mailSettings.SenderEmail, mailSettings.Password);
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