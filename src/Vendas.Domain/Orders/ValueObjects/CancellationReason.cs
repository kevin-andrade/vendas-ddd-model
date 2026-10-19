using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Domain.Orders;

public sealed class CancellationReason : ValueObject
{
    public string Code { get; }
    public string Description { get; }

    public CancellationReason(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("O código do motivo de cancelamento é obrigatório.");

        if (!_defaultReasons.ContainsKey(code))
            throw new DomainException($"Motivo de cancelamento '{code}' não é válido.");

        Code = code;
        Description = _defaultReasons[code];
    }
    
    // Set of standardized reasons in the domain
    private static readonly Dictionary<string, string> _defaultReasons = new()
    {
        { "CustomerCanceled", "Customer canceled the purchase" },
        { "PaymentError", "Error processing payment" },
        { "ItemOutOfStock", "Item out of stock" },
        { "InvalidAddress", "Invalid delivery address" },
        { "Other", "Other reason not specified" }
    };
    
    // Factory method for each common reason
    public static CancellationReason CustomerCanceled() => new("CustomerCanceled");
    public static CancellationReason PaymentError() => new("PaymentError");
    public static CancellationReason ItemOutOfStock() => new("ItemOutOfStock");
    public static CancellationReason InvalidAddress() => new("InvalidAddress");
    public static CancellationReason Other() => new("Other");
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
        yield return Description;
    }
}