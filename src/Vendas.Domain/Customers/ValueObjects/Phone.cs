using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Customers.ValueObjects;

public sealed class Phone : ValueObject
{
    public string Number { get; }

    public Phone(string number)
    {
        Guard.AgainstNullOrEmptyString(number, nameof(number), "Phone number is required.");

        var digits = new string(number.Where(char.IsDigit).ToArray());

        Guard.Against<DomainException>(
            digits.Length is < 10 or > 11,
            "Phone number must contain 10 (landline) or 11 digits (mobile).");

        Number = digits;
    }

    public override string ToString()
    {
        // Automatic formatting: (99) 99999-9999 or (99) 9999-9999
        if (Number.Length == 11)
            return Convert.ToUInt64(Number).ToString(@"\(00\) 00000\-0000");

        return Convert.ToUInt64(Number).ToString(@"\(00\) 0000\-0000");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Number;
    }
}