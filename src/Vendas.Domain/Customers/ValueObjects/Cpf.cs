using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Customers.ValueObjects;

public sealed class Cpf : ValueObject
{
    public string Number { get; }

    public Cpf(string number)
    {
        Guard.AgainstNullOrEmptyString(number, nameof(number), "CPF is required.");

        var digits = new string(number.Where(char.IsDigit).ToArray());

        Guard.Against<DomainException>(digits.Length != 11, "CPF must contain 11 digits.");
        Guard.Against<DomainException>(!IsValidCpf(digits), "Invalid CPF.");

        Number = digits;
    }

    public override string ToString()
        => Convert.ToUInt64(Number).ToString(@"000\.000\.000\-00");

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Number;
    }

    private static bool IsValidCpf(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;
        
        Span<char> digits = stackalloc char[cpf.Length];
        var digitCount = 0;

        foreach (char c in cpf)
        {
            if (char.IsDigit(c))
            {
                digits[digitCount++] = c;
            }
        }
        
        if (digitCount != 11)
            return false;
        
        ReadOnlySpan<char> cpfDigits = digits.Slice(0, 11);

        // Rejeita CPFs com todos os dígitos iguais (ex: "00000000000", "11111111111")
        bool allSame = true;
        for (int i = 1; i < 11; i++)
        {
            if (cpfDigits[i] != cpfDigits[0])
            {
                allSame = false;
                break;
            }
        }

        if (allSame)
            return false;
        
        int Sum(ReadOnlySpan<char> span, int length, int weight)
        {
            int sum = 0;
            for (int i = 0; i < length; i++)
                sum += (span[i] - '0') * (weight - i);
            return sum;
        }
        
        int dv1 = Sum(cpfDigits, 9, 10) % 11;
        dv1 = dv1 < 2 ? 0 : 11 - dv1;
        
        int dv2 = Sum(cpfDigits, 10, 11) % 11;
        dv2 = dv2 < 2 ? 0 : 11 - dv2;

        return cpfDigits[9] - '0' == dv1 && cpfDigits[10] - '0' == dv2;
    }
}