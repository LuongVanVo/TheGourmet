using TheGourmet.Domain.Entities.Identity;

namespace TheGourmet.Application.Common.Events;

public class OrderCreatedEvent
{
    public string Email { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}