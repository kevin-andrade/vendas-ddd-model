using MediatR;
using Vendas.Domain.Orders.ValueObjects;

namespace Vendas.Application.Orders.Commands.UpdateShippingAddress;

public sealed record UpdateShippingAddressCommand(
    Guid OrderId,
    ShippingAddress NewShippingAddress)  : IRequest<UpdateShippingAddressResultDto>;