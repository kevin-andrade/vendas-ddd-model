using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Catalog.ValueObjects;

public sealed class ProductName : ValueObject
{
    public string Value { get; }

    public ProductName(string value)
    {
        Guard.AgainstNullOrEmptyString(value, nameof(value),
            "Product name is required.");

        Guard.Against<DomainException>(value.Length < 3,
            "Product name must be at least 3 characters long.");

        Guard.Against<DomainException>(value.Length > 150,
            "Product name must be at most 150 characters long.");

        Value = value.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}