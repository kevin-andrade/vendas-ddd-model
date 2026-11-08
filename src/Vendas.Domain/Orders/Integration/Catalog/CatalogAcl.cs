using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Orders;

public sealed class CatalogAcl
{
    private readonly ICatalogGateway _gateway;

    public CatalogAcl(ICatalogGateway gateway)
    {
        Guard.AgainstNull(gateway, nameof(gateway));
        _gateway = gateway;
    }

    public async Task<ProductSnapshot> GetProductSnapshotAsync(Guid productId, CancellationToken ct = default)
    {
        var dto = await _gateway.GetProductByIdAsync(productId, ct)
                  ?? throw new DomainException("Product not found in the catalog");

        return new ProductSnapshot(dto.Id, dto.Name, dto.Price);
    }

    public async Task ValidateStockAsync(Guid productId, int quantity, CancellationToken ct = default)
    {
        var hasAvailableStock = await  _gateway.HasAvailableStockAsync(productId, quantity, ct);
        
        if (!hasAvailableStock)
            throw new DomainException("Insufficient stock for the product");
    }
}