using Vendas.Domain.Common.Base;
using Vendas.Domain.Orders.ValueObjects;

namespace Vendas.Domain.Orders.Events;

public sealed record OrderShippedEvent(
    Guid OrderId,
    Guid CustomerId,
    ShippingAddress ShippingAddress) : DomainEventBase;