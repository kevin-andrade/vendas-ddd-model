using Vendas.Domain.Orders;

namespace Vendas.Infrastructure.Fakes;

public sealed class FakeCatalogGateway : ICatalogGateway
{
    private static readonly Dictionary<Guid, ProductDto> _products = new()
    {
        [new Guid("11111111-0000-0000-0000-000000000001")] = new(new Guid("11111111-0000-0000-0000-000000000001"),
            "Gaming Laptop RTX 4060", 8500.00m),

        [new Guid("11111111-0000-0000-0000-000000000002")] = new(new Guid("11111111-0000-0000-0000-000000000002"),
            "Wireless Mouse Logitech MX Master", 450.00m),

        [new Guid("11111111-0000-0000-0000-000000000003")] = new(new Guid("11111111-0000-0000-0000-000000000003"),
            "Mechanical Keyboard Keychron K8", 680.00m),

        [new Guid("11111111-0000-0000-0000-000000000004")] = new(new Guid("11111111-0000-0000-0000-000000000004"),
            "34-Inch Ultrawide Monitor", 3200.00m),
    };

    public Task<ProductDto?> GetProductByIdAsync(Guid productId, CancellationToken ct = default)
    {
        _products.TryGetValue(productId, out var product);
        return Task.FromResult(product);
    }
}