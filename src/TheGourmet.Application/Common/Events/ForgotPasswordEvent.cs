namespace TheGourmet.Application.Common.Events
{
    public class ForgotPasswordEvent
    {
        public string Email { get; set; } = string.Empty;
        public string ResetLink { get; set; } = string.Empty;
    }
}