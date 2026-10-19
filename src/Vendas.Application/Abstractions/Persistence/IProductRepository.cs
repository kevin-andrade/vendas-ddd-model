using Vendas.Domain.Catalog;

namespace Vendas.Application.Abstractions.Persistence;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid productId,
        CancellationToken cancellationToken = default);

    Task AddAsync(Product product,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(Product product,
        CancellationToken cancellationToken = default);
}