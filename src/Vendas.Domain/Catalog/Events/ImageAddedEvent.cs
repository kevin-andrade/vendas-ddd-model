using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Catalog.Events;

public sealed record ImageAddedEvent(Guid ProductId, string Url, int Order) : DomainEventBase;