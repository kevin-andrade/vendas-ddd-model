using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Orders;

public sealed class OrderItem : Entity
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal DiscountApplied { get; private set; }
    public decimal TotalPrice { get; private set; }
    
    internal OrderItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        Guard.AgainstEmptyGuid(productId, nameof(productId));
        Guard.AgainstNullOrEmptyString(productName, nameof(ProductName));
        Guard.Against<DomainException>(unitPrice <= 0, "O Preço unitário deve ser maior do que zero");
        Guard.Against<DomainException>(quantity <= 0, "A quantidade deve ser maior do que zero");
        
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;

        CalculateTotalAmount();
    }

    public void ApplyDiscount(decimal discount)
    {
        Guard.Against<DomainException>(discount < 0, "O desconto não pode ser negativo");
        Guard.Against<DomainException>(discount > UnitPrice * Quantity, "Desconto não pode exceder o valor total do item");
        
        DiscountApplied = discount;
        SetUpdatedAt();
        CalculateTotalAmount();
    }

    public void AddUnits(decimal units)
    {
        Guard.Against<DomainException>(units < 0, "O valor deve ser maior do que zero");

        Quantity += units;
        SetUpdatedAt();
        CalculateTotalAmount();
    }

    public void RemoveUnits(decimal units)
    {
        Guard.Against<DomainException>(units <= 0, "Deve-se remover pelo menos uma unidade");
        Guard.Against<DomainException>(units > Quantity, "Não é possível remover mais unidades do que existem no item");
        
        Quantity -= units;
        Guard.Against<DomainException>(Quantity == 0, "Um item de pedido não pode ter quantidade zero, remova no Pedido");
        
        SetUpdatedAt();
        CalculateTotalAmount();
    }

    public void UpdateUnitPrice(decimal newPrice)
    {
        Guard.Against<DomainException>(newPrice <= 0, "O preço unitário deve ser maior do que zero");

        UnitPrice = newPrice;
        SetUpdatedAt();
        CalculateTotalAmount();
    }

    private void CalculateTotalAmount()
    {
        TotalPrice = (UnitPrice * Quantity) - DiscountApplied;
    }
}

