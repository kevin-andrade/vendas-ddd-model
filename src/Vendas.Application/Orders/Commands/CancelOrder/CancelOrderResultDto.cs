namespace Vendas.Application.Orders.Commands.CancelOrder;

public sealed record CancelOrderResultDto(
    Guid OrderId,
    string Status);