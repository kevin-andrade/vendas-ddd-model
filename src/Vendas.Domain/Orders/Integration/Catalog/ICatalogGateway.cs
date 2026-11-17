namespace Vendas.Domain.Orders;

public interface ICatalogGateway
{
    Task<ProductDto?> GetProductByIdAsync(Guid productId, CancellationToken ct = default);
}