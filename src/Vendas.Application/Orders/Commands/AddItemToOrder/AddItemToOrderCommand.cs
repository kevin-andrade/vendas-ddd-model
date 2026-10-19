using MediatR;

namespace Vendas.Application.Orders.Commands.AddItemToOrder;

public sealed record AddItemToOrderCommand(
    Guid OrderId,
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity) :IRequest<AddItemToOrderResultDto>;