using MediatR;

namespace Vendas.Application.Orders.Commands.RemoveItemFromOrder;

public sealed record RemoveItemFromOrderCommand(
    Guid OrderId,
    Guid ProductId)  : IRequest<RemoveItemFromOrderResultDto>;