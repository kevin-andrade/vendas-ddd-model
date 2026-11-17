using MediatR;

namespace Vendas.Application.Orders.Commands.AddItemToOrder;

public sealed record AddItemToOrderCommand(
    Guid OrderId,
    Guid ProductId,
    int Quantity) :IRequest<AddItemToOrderResultDto>;