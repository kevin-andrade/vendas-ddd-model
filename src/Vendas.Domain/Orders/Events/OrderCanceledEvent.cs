using Vendas.Domain.Common.Base;
using Vendas.Domain.Orders.Enums;
using Vendas.Domain.Orders.ValueObjects;

namespace Vendas.Domain.Orders.Events;

public sealed record OrderCanceledEvent(
    Guid OrderId,
    Guid CustomerId,
    OrderStatus PreviousStatus,
    CancellationReason Reason,
    Guid? PaymentId) : DomainEventBase;