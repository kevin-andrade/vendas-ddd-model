namespace Vendas.Domain.Orders;

public interface ICatalogGateway
{
    Task<ProductDto?> GetProductByIdAsync(Guid productId, CancellationToken ct = default);
    
    Task<bool> HasAvailableStockAsync(Guid productId, int quantity, CancellationToken ct = default);
}