namespace Vendas.Domain.Orders.Enums;

public enum OrderStatus
{
  Pending = 1,
  PaymentConfirmed = 2,
  Preparing = 3,
  Shipped = 4,
  Delivered = 5,
  Canceled = 6,
}