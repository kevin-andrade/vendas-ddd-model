using System.Text.RegularExpressions;
using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Customers.ValueObjects;

public sealed class Email : ValueObject
{
    public string Address { get; }

    private static readonly Regex _regex = new(
        @"^[\w\.-]+@[\w\.-]+\.\w{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public Email(string address)
    {
        Guard.AgainstNullOrEmptyString(address, nameof(address), "Email is required.");
        Guard.Against<DomainException>(!_regex.IsMatch(address), "Invalid email.");

        Address = address.Trim().ToLowerInvariant();
    }

    public override string ToString() => Address;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Address;
    }
}