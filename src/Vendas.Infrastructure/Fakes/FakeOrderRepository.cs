using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Orders;

namespace Vendas.Infrastructure.Fakes;

public sealed class FakeOrderRepository : IOrderRepository
{
    private readonly Dictionary<Guid, Order> _orders = new();
    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            _orders.TryGetValue(orderId, out var order);
            return order;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            _orders[order.Id] = order;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (!_orders.ContainsKey(order.Id))
                throw new InvalidOperationException(
                    $"Order {order.Id} not found for update.");

            _orders[order.Id] = order;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<IReadOnlyList<Order>> ListAllAsync(CancellationToken  cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            return _orders.Values.ToList().AsReadOnly();
        }
        finally
        {
            _lock.Release();
        }
    }
}
