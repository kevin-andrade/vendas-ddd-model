namespace Vendas.Application.Orders.Commands.MarkAsShipped;

public sealed record MarkAsShippedResultDto(
    Guid OrderId,
    string Status);