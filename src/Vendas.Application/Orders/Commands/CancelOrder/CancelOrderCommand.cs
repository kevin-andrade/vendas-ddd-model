using MediatR;

namespace Vendas.Application.Orders.Commands.CancelOrder;

public sealed record CancelOrderCommand(
    Guid  OrderId,
    string ReasonCode) : IRequest<CancelOrderResultDto>;