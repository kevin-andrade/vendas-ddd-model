using MediatR;

namespace Vendas.Application.Orders.Commands.MarkAsPreparing;

public sealed record MarkAsPreparingCommand(Guid OrderId) : IRequest<MarkAsPreparingResultDto>;