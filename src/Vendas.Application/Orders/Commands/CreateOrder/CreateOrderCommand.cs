using MediatR;
using Vendas.Domain.Orders.ValueObjects;

namespace Vendas.Application.Orders.Commands.CreateOrder;

public sealed record CreateOrderCommand(
    Guid CustomerId,
    ShippingAddress DeliveryAddress) : IRequest<CreateOrderResultDto>;