using MediatR;

namespace Vendas.Application.Orders.Commands.CreateOrder;

public sealed record CreateOrderCommand(
    Guid CustomerId,
    Guid AddressId) : IRequest<CreateOrderResultDto>;