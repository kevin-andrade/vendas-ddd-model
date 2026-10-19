using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Orders.Events;

public sealed record OrderDeliveredEvent(
    Guid OrderId,
    Guid CustomerId) : DomainEventBase;