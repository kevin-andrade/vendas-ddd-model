using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Catalog.ValueObjects;

public sealed class ProductPrice : ValueObject
{
    public decimal Value { get; }

    public ProductPrice(decimal value)
    {
        Guard.Against<DomainException>(value <= 0,
            "Product price must be greater than zero.");

        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}