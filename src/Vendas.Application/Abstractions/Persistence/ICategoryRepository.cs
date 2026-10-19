using Vendas.Domain.Catalog;

namespace Vendas.Application.Abstractions.Persistence;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid categoryId,
        CancellationToken cancellationToken = default);

    Task AddAsync(Category category,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(Category category,
        CancellationToken cancellationToken = default);
}