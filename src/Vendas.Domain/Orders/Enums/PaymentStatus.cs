namespace Vendas.Domain.Orders.Enums;

public enum PaymentStatus
{
    Pending = 1,
    Approved = 2,
    Declined = 3,
    Refunded = 4,
    Canceled = 5,
}