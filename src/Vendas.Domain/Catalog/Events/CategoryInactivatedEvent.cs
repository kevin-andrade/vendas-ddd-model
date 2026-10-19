using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Catalog.Events;

public sealed record CategoryInactivatedEvent(Guid CategoryId) : DomainEventBase;