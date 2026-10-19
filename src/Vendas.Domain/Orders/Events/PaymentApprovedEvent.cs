using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Orders.Events;

public record PaymentApprovedEvent(
    Guid PaymentId,
    Guid OrderId,
    decimal Amount,
    DateTime PaidAt,
    string? TransactionCode) : DomainEventBase;