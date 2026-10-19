namespace Vendas.Application.Orders.Commands.MarkAsPreparing;

public sealed record MarkAsPreparingResultDto(
    Guid OrderId,
    string Status);