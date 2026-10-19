using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Catalog.ValueObjects;

public sealed class ProductImage : ValueObject
{
    public string Url { get; }
    public int Order { get; }

    public ProductImage(string url, int displayOrder)
    {
        Guard.AgainstNullOrEmptyString(url, nameof(url),
            "Image URL is required.");

        Guard.Against<DomainException>(displayOrder < 1,
            "Image display order must be greater than or equal to 1.");

        Url = url;
        Order = displayOrder;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Url;
        yield return Order;
    }
}