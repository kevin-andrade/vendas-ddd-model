using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Customers.Events;

public sealed record PrimaryAddressChangedEvent(
    Guid CustomerId,
    Guid NewAddressId) : DomainEventBase;