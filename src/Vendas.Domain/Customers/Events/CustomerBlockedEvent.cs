using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Customers.Events;

public sealed record CustomerBlockedEvent(
    Guid CustomerId,
    string Cpf) : DomainEventBase;