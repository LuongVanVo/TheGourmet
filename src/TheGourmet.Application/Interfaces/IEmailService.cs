namespace TheGourmet.Application.Interfaces
{
    public interface IEmailService
    {
        // Hàm gửi mail: Gửi cho ai, Tiêu đề là gì, Nội dung là gì
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
    }
}