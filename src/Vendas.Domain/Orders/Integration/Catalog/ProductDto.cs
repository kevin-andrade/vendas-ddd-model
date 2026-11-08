namespace Vendas.Domain.Orders;

public sealed record ProductDto(
    Guid Id,
    string Name,
    decimal Price);