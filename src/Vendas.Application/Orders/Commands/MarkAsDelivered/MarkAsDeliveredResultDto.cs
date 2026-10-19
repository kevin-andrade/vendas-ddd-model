namespace Vendas.Application.Orders.Commands.MarkAsDelivered;

public sealed record MarkAsDeliveredResultDto(Guid OrderId, string Status);