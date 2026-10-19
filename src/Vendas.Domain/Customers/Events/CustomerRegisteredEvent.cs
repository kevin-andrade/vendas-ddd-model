using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Customers.Events;

public sealed record CustomerRegisteredEvent(
    Guid CustomerId,
    string Name,
    string Cpf,
    string Email) : DomainEventBase;