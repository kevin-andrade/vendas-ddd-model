
namespace Vendas.Domain.Orders;

public sealed class CatalogAcl
{
    public (string nameProduct, decimal unitPrice) MapProduct(ProductDto dto)
    {
        return (dto.Name, dto.Price);
    }
}