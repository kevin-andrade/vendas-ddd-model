namespace Vendas.Application.Orders.Commands.RemoveItemFromOrder;

public sealed record RemoveItemFromOrderResultDto(
    Guid OrderId,
    decimal TotalAmount,
    string Status);