using Vendas.Domain.Customers;

namespace Vendas.Application.Abstractions.Persistence;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid customerId,
        CancellationToken cancellationToken = default);

    Task AddAsync(Customer customer,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(Customer customer,
        CancellationToken cancellationToken = default);
}