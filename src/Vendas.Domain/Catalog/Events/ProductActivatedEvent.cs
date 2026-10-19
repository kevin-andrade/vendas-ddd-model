using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Catalog.Events;

public sealed record ProductActivatedEvent(Guid ProductId) : DomainEventBase;