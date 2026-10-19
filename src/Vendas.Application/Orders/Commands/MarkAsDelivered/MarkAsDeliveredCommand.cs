using MediatR;

namespace Vendas.Application.Orders.Commands.MarkAsDelivered;

public sealed record MarkAsDeliveredCommand(
    Guid OrderId,
    string Status) : IRequest<MarkAsDeliveredResultDto>;