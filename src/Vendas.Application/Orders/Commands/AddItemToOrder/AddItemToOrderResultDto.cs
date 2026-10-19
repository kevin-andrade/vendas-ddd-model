namespace Vendas.Application.Orders.Commands.AddItemToOrder;

public sealed record AddItemToOrderResultDto(
    Guid OrderId,
    decimal TotalAmount,
    string Status);