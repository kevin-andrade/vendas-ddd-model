using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Catalog.Events;

public sealed record ProductInactivatedEvent(Guid ProductId) : DomainEventBase;