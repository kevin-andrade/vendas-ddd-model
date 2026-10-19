using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Catalog.ValueObjects;

public sealed class ProductCode : ValueObject
{
    public string Value { get; }

    public ProductCode(string value)
    {
        Guard.AgainstNullOrEmptyString(value, nameof(value),
            "Product code is required.");

        Guard.Against<DomainException>(value.Length < 3,
            "Product code must be at least 3 characters long.");

        Value = value.Trim().ToUpper();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}