using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;
using Vendas.Domain.Orders.Enums;
using Vendas.Domain.Orders.Events;

namespace Vendas.Domain.Orders;

public sealed class Payment : Entity
{
    public Guid OrderId { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public string? TransactionCode { get; private set; }

    public Payment(Guid orderId, PaymentMethod paymentMethod, decimal amount)
    {
        Guard.AgainstEmptyGuid(orderId, nameof(orderId), "Pedido Inválido.");
        Guard.Against<DomainException>(amount <= 0, "O Amount do pagamento deve ser maior que zero.");
        Guard.Against<DomainException>(
            !Enum.IsDefined(typeof(PaymentMethod), paymentMethod),
            "Método de pagamento inválido.");
        
        OrderId = orderId;
        PaymentMethod = paymentMethod;
        Amount = amount;
        
        //Status inicial do Pagamento
        PaymentStatus = PaymentStatus.Pending;
        PaidAt = null;
        TransactionCode = null;
    }
    
    public void GenerateTransactionCodeLocal()
    {
        if (TransactionCode is not null)
            return;

        var code = $"LOCAL-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        SetTransactionCode(code);
    }

    public void SetTransactionCode(string code)
    {
        Guard.AgainstNullOrEmptyString(code, nameof(code), "Código da transação inválido.");
        Guard.Against<DomainException>(TransactionCode is not null, "O código de transação já foi definido.");
        Guard.Against<DomainException>(
            PaymentStatus != PaymentStatus.Pending,
            "Não é permitido registrar código após confirmação ou recusa do pagamento.");

        // gerado apenas uma vez, quando o pagamento é aprovado
        TransactionCode = code;
        SetUpdatedAt();
    }
    
    public void ConfirmPayment()
    {
        Guard.Against<DomainException>(
            PaymentStatus != PaymentStatus.Pending,
            "Apenas pagamentos Pendings podem ser confirmados.");

        Guard.AgainstNullOrEmptyString(
            TransactionCode ?? string.Empty,
            nameof(TransactionCode),
            "O pagamento não pode ser confirmado sem o código de transação.");

        PaymentStatus = PaymentStatus.Approved;
        PaidAt = DateTime.UtcNow;
        SetUpdatedAt();

        AddDomainEvent(new PaymentApprovedEvent(Id,
            OrderId,
            Amount,
            PaidAt.Value,
            TransactionCode));
    }

    public void RecusarPagamento()
    {
        Guard.Against<DomainException>(
            PaymentStatus != PaymentStatus.Pending,
            "Apenas pagamentos Pendings podem ser recusados.");

        PaymentStatus = PaymentStatus.Declined;
        PaidAt = DateTime.UtcNow;
        SetUpdatedAt();

        AddDomainEvent(new PaymentDeclinedEvent(Id,
            OrderId,
            Amount,
            PaidAt.Value,
            TransactionCode));
    }
}