using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Catalog.Events;

public sealed record StockAdjustedEvent(Guid ProductId, int Quantity, string Reason) : DomainEventBase;