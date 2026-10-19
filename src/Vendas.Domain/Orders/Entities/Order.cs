using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;
using Vendas.Domain.Orders.Enums;
using Vendas.Domain.Orders.Events;
using Vendas.Domain.Orders.ValueObjects;

namespace Vendas.Domain.Orders;

public sealed class Order : AggregateRoot
{
    // ===== Domain Properties =====
    public Guid CustomerId { get; private set; }
    public ShippingAddress ShippingAddress { get; private set; }
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    public string OrderNumber { get; private set; } = string.Empty;

    // ===== Items =====
    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    // ===== Payments =====
    private readonly List<Payment> _payments = new();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    private Order(Guid customerId, ShippingAddress shippingAddress)
    {
        Guard.AgainstEmptyGuid(customerId, nameof(customerId), "Invalid CustomerId.");
        Guard.AgainstNull(shippingAddress, nameof(shippingAddress), "Shipping address is required.");

        CustomerId = customerId;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Pending;
        TotalAmount = 0m;

        GenerateOrderNumber();
    }

    public static Order Create(Guid customerId, ShippingAddress shippingAddress)
        => new(customerId, shippingAddress);

    public void AddItem(Guid productId, string productName,
        decimal unitPrice, int quantity)
    {
        Guard.Against<DomainException>(
            Status != OrderStatus.Pending,
            "Items can only be added while the order is pending.");

        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null)
            existing.AddUnits(quantity);
        else
            _items.Add(new OrderItem(productId, productName, unitPrice, quantity));

        RecalculateTotalAmount();
        SetUpdatedAt();
    }

    public void RemoveItem(Guid itemId)
    {
        Guard.AgainstEmptyGuid(itemId, nameof(itemId), "Invalid ItemId.");
        Guard.Against<DomainException>(
            Status != OrderStatus.Pending,
            "Items can only be removed from pending orders.");

        var item = _items.FirstOrDefault(i => i.Id == itemId);
        Guard.AgainstNull(item, nameof(item), "Item not found in order.");

        _items.Remove(item!);

        Guard.Against<DomainException>(_items.Count == 0,
            "The order must contain at least one item.");

        RecalculateTotalAmount();
        SetUpdatedAt();
    }

    public void UpdateShippingAddress(ShippingAddress newAddress)
    {
        Guard.AgainstNull(newAddress, nameof(newAddress));
        Guard.Against<DomainException>(
            Status != OrderStatus.Pending,
            "Shipping address can only be changed while the order is pending.");

        ShippingAddress = newAddress;
        SetUpdatedAt();
    }

    public Payment StartPayment(PaymentMethod paymentMethod)
    {
        Guard.Against<DomainException>(!_items.Any(),
            "Cannot start payment for an order with no items.");

        Guard.Against<DomainException>(Status != OrderStatus.Pending,
            "Payment can only be started from Pending status.");

        if (_payments.Any(p => p.PaymentStatus == PaymentStatus.Pending))
            throw new DomainException("There is already a pending payment for this order.");

        var newPayment = new Payment(Id, paymentMethod, TotalAmount);
        _payments.Add(newPayment);
        SetUpdatedAt();

        // Payment itself will publish PaymentApprovedEvent or PaymentRejectedEvent.
        return newPayment;
    }

    public void HandlePaymentApproved(Guid paymentId)
    {
        var payment = _payments.FirstOrDefault(p => p.Id == paymentId);
        if (payment is null) return;

        Guard.Against<DomainException>(
            Status != OrderStatus.Pending,
            "The order is not in the expected status for payment confirmation.");

        Status = OrderStatus.PaymentConfirmed;
        SetUpdatedAt();
        
        AddDomainEvent(new PaymentApprovedEvent(payment.Id, payment.OrderId, payment.Amount, payment.PaidAt ?? DateTime.Now, payment.TransactionCode));
    }

    // Reacts to PaymentRejectedEvent (internal to the same BC)
    public void HandlePaymentRejected(Guid paymentId)
    {
        var payment = _payments.FirstOrDefault(p => p.Id == paymentId);
        if (payment is null) return;

        Guard.Against<DomainException>(
            Status != OrderStatus.Pending,
            "The order is not in the expected status for payment rejection.");

        Status = OrderStatus.Canceled;
        SetUpdatedAt();

        AddDomainEvent(new OrderCanceledEvent(
            Id,
            CustomerId,
            Status,
            CancellationReason.PaymentError(),
            payment.Id));
    }

    public void MarkAsPreparing()
    {
        Guard.Against<DomainException>(
            Status != OrderStatus.PaymentConfirmed,
            "The order can only move to 'Preparing' after payment confirmation.");

        Status = OrderStatus.Preparing;
        SetUpdatedAt();
    }

    public void MarkAsShipped()
    {
        Guard.Against<DomainException>(
            Status != OrderStatus.Preparing,
            "The order can only be 'Shipped' after being 'Preparing'.");

        Status = OrderStatus.Shipped;
        SetUpdatedAt();

        AddDomainEvent(new OrderShippedEvent(Id, CustomerId, ShippingAddress));
    }

    public void MarkAsDelivered()
    {
        Guard.Against<DomainException>(
            Status != OrderStatus.Shipped,
            "The order can only be marked as 'Delivered' after being 'Shipped'.");

        Status = OrderStatus.Delivered;
        SetUpdatedAt();

        AddDomainEvent(new OrderDeliveredEvent(Id, CustomerId));
    }

    public void CancelOrder(CancellationReason? reason = null)
    {
        Guard.Against<DomainException>(
            Status >= OrderStatus.Preparing,
            "Cannot cancel an order that is already in preparation or beyond.");

        Status = OrderStatus.Canceled;
        SetUpdatedAt();

        AddDomainEvent(new OrderCanceledEvent(
            Id,
            CustomerId,
            Status,
            reason ?? CancellationReason.Other(),
            _payments.LastOrDefault()?.Id));
    }

    private void RecalculateTotalAmount()
        => TotalAmount = _items.Sum(i => i.TotalPrice);

    private void GenerateOrderNumber()
        => OrderNumber = $"ORD-{Id.ToString()[..8].ToUpper()}";
}