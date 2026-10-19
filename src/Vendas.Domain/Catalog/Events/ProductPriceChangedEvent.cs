using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Catalog.Events;

public sealed record ProductPriceChangedEvent(Guid ProductId, decimal OldPrice, decimal NewPrice) : DomainEventBase;