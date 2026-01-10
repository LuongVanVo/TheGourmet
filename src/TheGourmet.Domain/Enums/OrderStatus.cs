namespace TheGourmet.Domain.Enums;

public enum OrderStatus
{
    // Order has been created, but not yet paid
    Pending = 1,
    // Order has been paid
    Paid = 2,
    // Order has been cancelled
    Cancelled = 3,
    // Order has out of validity period (backgound job to scan this status)
    Expired = 4,
    // Order has been completed
    Completed = 5
}