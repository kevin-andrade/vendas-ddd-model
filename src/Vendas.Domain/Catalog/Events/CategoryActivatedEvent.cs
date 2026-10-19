using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Catalog.Events;

public sealed record CategoryActivatedEvent(Guid CategoryId) : DomainEventBase;