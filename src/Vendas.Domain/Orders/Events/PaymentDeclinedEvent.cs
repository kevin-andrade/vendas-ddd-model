using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Orders.Events;

public record PaymentDeclinedEvent(
    Guid PaymentId,
    Guid OrderId,
    decimal Amount,
    DateTime PaidAt,
    string? TransactionCode) : DomainEventBase;