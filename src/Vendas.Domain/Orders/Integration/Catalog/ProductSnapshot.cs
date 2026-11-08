using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Orders;

public sealed class ProductSnapshot : ValueObject
{
    public Guid ProductId { get; }
    public string ProductName { get; }
    public decimal UnitPrice { get; }

    public ProductSnapshot(Guid productId, string productName, decimal unitPrice)
    {
        Guard.AgainstEmptyGuid(productId, nameof(productId), "Invalid product ID.");
        Guard.AgainstNullOrEmptyString(productName, nameof(productName), "Product name cannot be empty.");
        Guard.Against<DomainException>(unitPrice <= 0, "Invalid unit price.");

        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ProductId;
        yield return ProductName;
        yield return UnitPrice;
    }
}