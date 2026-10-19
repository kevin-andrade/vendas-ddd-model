using MediatR;

namespace Vendas.Application.Orders.Commands.MarkAsShipped;

public sealed record MarkAsShippedCommand(
    Guid OrderId) : IRequest<MarkAsShippedResultDto>;