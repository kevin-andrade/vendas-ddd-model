namespace Vendas.Application.Orders.Commands.CreateOrder;

public sealed record CreateOrderResultDto(
    Guid Id,
    string OrderNumber,
    DateTime CreatedAt,
    decimal TotalAmount,
    string OrderStatus);