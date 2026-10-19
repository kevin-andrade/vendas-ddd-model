using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Customers.ValueObjects;

public sealed class FullName : ValueObject
{
    public string FirstName { get; }
    public string LastName { get; }
    public string FormattedFullName { get; }

    public FullName(string fullName)
    {
        Guard.AgainstNullOrEmptyString(fullName, nameof(fullName),
            "Full name is required.");

        var parts = fullName
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        Guard.Against<DomainException>(parts.Length < 2,
            "Full name must contain at least a first name and a last name.");

        LastName = parts.Last();
        FirstName = string.Join(" ", parts.Take(parts.Length - 1));

        FormattedFullName = string.Join(" ", parts);
    }

    public string ShortName => $"{FirstName.Split(' ').First()} {LastName}";

    public override string ToString() => FormattedFullName;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FormattedFullName.ToLowerInvariant();
    }
}