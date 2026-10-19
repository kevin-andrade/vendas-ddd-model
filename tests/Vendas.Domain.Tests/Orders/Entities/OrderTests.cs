using System.Reflection;
using FluentAssertions;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Orders;
using Vendas.Domain.Orders.Enums;
using Vendas.Domain.Orders.Events;
using Vendas.Domain.Orders.ValueObjects;

namespace Vendas.Domain.Tests.Orders.Entities;

public class OrderTests
{
    private static ShippingAddress CreateValidAddress()
      => ShippingAddress.Create("12345-000", "Rua A", "Ap 1", "Centro", "SP", "São Paulo", "Brasil");

    private static readonly Guid ValidCustomerId = Guid.NewGuid();
    private static readonly Guid ValidProductId = Guid.NewGuid();

    private static void SetOrderStatus(Order order, OrderStatus status)
    {
        typeof(Order).GetProperty(nameof(Order.Status), BindingFlags.Public | BindingFlags.Instance)!
                     .SetValue(order, status);
    }

    // Order Creation

    [Fact(DisplayName = "Should create valid order with Pending status")]
    public void Should_Create_Valid_Order()
    {
        // Act
        var order = Order.Create(ValidCustomerId, CreateValidAddress());

        // Assert
        order.Should().NotBeNull();
        order.CustomerId.Should().Be(ValidCustomerId);
        order.ShippingAddress.Should().NotBeNull();
        order.Status.Should().Be(OrderStatus.Pending);
        order.TotalAmount.Should().Be(0);
        order.Items.Should().BeEmpty();
        order.Payments.Should().BeEmpty();
        order.Id.Should().NotBeEmpty();
    }

    [Fact(DisplayName = "Should not create order with invalid CustomerId")]
    public void Should_Not_Create_Order_With_Invalid_CustomerId()
    {
        // Act
        Action act = () => Order.Create(Guid.Empty, CreateValidAddress());

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Invalid CustomerId.");
    }

    [Fact(DisplayName = "Should not create order without shipping address")]
    public void Should_Not_Create_Order_Without_Address()
    {
        // Act
        Action act = () => Order.Create(ValidCustomerId, null!);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Shipping address is required.");
    }

    // Items

    [Fact(DisplayName = "Should add item to order and recalculate total amount")]
    public void Should_Add_Item_To_Order()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());

        // Act
        order.AddItem(ValidProductId, "Mouse", 100m, 2);

        // Assert
        order.Items.Should().HaveCount(1);
        order.TotalAmount.Should().Be(200m);
        order.Items.First().TotalPrice.Should().Be(200m);
    }

    [Fact(DisplayName = "Should sum quantity of existing item when adding same product")]
    public void Should_Sum_Quantity_Of_Existing_Item()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        var productId = ValidProductId;

        // Act
        order.AddItem(productId, "Teclado", 200m, 1);
        order.AddItem(productId, "Teclado", 200m, 2);

        // Assert
        order.Items.Should().HaveCount(1);
        var item = order.Items.First();
        item.Quantity.Should().Be(3);
        item.TotalPrice.Should().Be(600m);
        order.TotalAmount.Should().Be(600m);
    }

    [Theory(DisplayName = "Should not allow adding items when order is not Pending")]
    [InlineData(OrderStatus.PaymentConfirmed)]
    [InlineData(OrderStatus.Preparing)]
    [InlineData(OrderStatus.Shipped)]
    [InlineData(OrderStatus.Delivered)]
    [InlineData(OrderStatus.Canceled)]
    public void Should_Not_Add_Item_When_Order_Is_Not_Pending(OrderStatus status)
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        SetOrderStatus(order, status);

        // Act
        Action act = () => order.AddItem(Guid.NewGuid(), "Outro", 100m, 1);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Items can only be added while the order is pending.");
    }

    [Fact(DisplayName = "Should fail to remove item when it leaves order empty")]
    public void Should_Fail_To_Remove_Item_When_It_Leaves_Order_Empty()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        order.AddItem(ValidProductId, "Mouse", 100m, 2);

        // Act
        Action act = () => order.RemoveItem(order.Items.First().Id);

        // Assert
        act.Should().Throw<DomainException>()
          .WithMessage("The order must contain at least one item.");
    }

    [Fact(DisplayName = "Should remove item and recalculate total amount when more than one item exists")]
    public void Should_Remove_Item_When_More_Than_One_Exists()
    {
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        var product1 = Guid.NewGuid();
        var product2 = Guid.NewGuid();

        order.AddItem(product1, "Mouse", 100m, 1);
        order.AddItem(product2, "Teclado", 200m, 1);

        var itemId = order.Items.First(i => i.ProductId == product1).Id;
        order.RemoveItem(itemId);

        order.Items.Should().HaveCount(1);
        order.TotalAmount.Should().Be(200m);
    }

    [Fact(DisplayName = "Should throw exception when attempting to remove non-existing item")]
    public void Should_Throw_When_Removing_Non_Existing_Item()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        order.AddItem(ValidProductId, "Mouse", 100m, 2);

        // Act
        Action act = () => order.RemoveItem(Guid.NewGuid());

        // Assert
        act.Should().Throw<DomainException>()
                    .WithMessage("Item not found in order.");
    }

    [Theory(DisplayName = "Should not allow removing items when order is not Pending")]
    [InlineData(OrderStatus.PaymentConfirmed)]
    [InlineData(OrderStatus.Preparing)]
    [InlineData(OrderStatus.Shipped)]
    [InlineData(OrderStatus.Delivered)]
    [InlineData(OrderStatus.Canceled)]
    public void Should_Not_Remove_Item_When_Not_Pending(OrderStatus status)
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        order.AddItem(ValidProductId, "Produto", 10m, 1);
        SetOrderStatus(order, status);

        // Act
        Action act = () => order.RemoveItem(ValidProductId);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Items can only be removed from pending orders.");
    }

    // Address

    [Fact(DisplayName = "Should update shipping address when Pending")]
    public void Should_Update_Shipping_Address_When_Pending()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        var newAddress = ShippingAddress.Create("00000-000", "Nova Rua", "Casa", "Bairro Novo", "RJ", "Rio", "Brasil");

        // Act
        order.UpdateShippingAddress(newAddress);

        // Assert
        order.ShippingAddress.Should().Be(newAddress);
    }

    [Theory(DisplayName = "Should not update shipping address when not Pending")]
    [InlineData(OrderStatus.PaymentConfirmed)]
    [InlineData(OrderStatus.Preparing)]
    [InlineData(OrderStatus.Shipped)]
    [InlineData(OrderStatus.Delivered)]
    [InlineData(OrderStatus.Canceled)]
    public void Should_Not_Update_Shipping_Address_When_Not_Pending(OrderStatus status)
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        var newAddress = ShippingAddress.Create("00000-000", "Nova Rua", "Casa", "Bairro Novo", "RJ", "Rio", "Brasil");
        SetOrderStatus(order, status);

        // Act
        Action act = () => order.UpdateShippingAddress(newAddress);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Shipping address can only be changed while the order is pending.");
    }

    // Payments

    [Fact(DisplayName = "Should start payment and keep status Pending")]
    public void Should_Start_Payment()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        order.AddItem(ValidProductId, "Produto", 100m, 2);

        // Act
        var payment = order.StartPayment(PaymentMethod.CreditCard);

        // Assert
        payment.Should().NotBeNull();
        payment.Amount.Should().Be(200m);
        order.Payments.Should().Contain(payment);
        order.Status.Should().Be(OrderStatus.Pending);
    }

    [Fact(DisplayName = "Should not start payment without items in order")]
    public void Should_Not_Start_Payment_Without_Items()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());

        // Act
        Action act = () => order.StartPayment(PaymentMethod.Pix);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot start payment for an order with no items.");
    }

    [Fact(DisplayName = "Should not start payment if there is already a Pending payment")]
    public void Should_Not_Start_Payment_If_Already_Has_Pending()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        order.AddItem(ValidProductId, "Produto", 100m, 1);

        order.StartPayment(PaymentMethod.Pix);

        // Act
        Action act = () => order.StartPayment(PaymentMethod.CreditCard);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("There is already a pending payment for this order.");
    }

    [Fact(DisplayName = "Should change status to PaymentConfirmed on HandlePaymentApproved")]
    public void Should_Change_Status_On_HandlePaymentApproved()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        order.AddItem(ValidProductId, "Produto", 100m, 1);
        var payment = order.StartPayment(PaymentMethod.Pix);

        // Act
        order.HandlePaymentApproved(payment.Id);

        // Assert
        order.Status.Should().Be(OrderStatus.PaymentConfirmed);
    }

    [Fact(DisplayName = "Should cancel order and emit Canceled event on HandlePaymentRejected")]
    public void Should_Cancel_Order_On_HandlePaymentRejected()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        order.AddItem(ValidProductId, "Produto", 100m, 1);
        var payment = order.StartPayment(PaymentMethod.Pix);

        // Act
        order.HandlePaymentRejected(payment.Id);

        // Assert
        order.Status.Should().Be(OrderStatus.Canceled);
        order.DomainEvents.Should().ContainSingle(e => e is OrderCanceledEvent);
    }

    [Fact(DisplayName = "Should not HandlePaymentApproved if status is not Pending")]
    public void Should_Not_HandlePaymentApproved_If_Not_Pending()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        order.AddItem(ValidProductId, "Produto", 100m, 1);
        var payment = order.StartPayment(PaymentMethod.Pix);
        SetOrderStatus(order, OrderStatus.Preparing);

        // Act
        Action act = () => order.HandlePaymentApproved(payment.Id);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("The order is not in the expected status for payment confirmation.");
    }

    // State Transitions

    [Fact(DisplayName = "Should allow marking order as Preparing after PaymentConfirmed")]
    public void Should_Mark_As_Preparing()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        order.AddItem(ValidProductId, "Produto", 100m, 1);
        var payment = order.StartPayment(PaymentMethod.CreditCard);
        order.HandlePaymentApproved(payment.Id); // Status: PaymentConfirmed

        // Act
        order.MarkAsPreparing();

        // Assert
        order.Status.Should().Be(OrderStatus.Preparing);
    }

    [Fact(DisplayName = "Should not mark as Preparing if not PaymentConfirmed")]
    public void Should_Not_Mark_As_Preparing_If_Not_Confirmed()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());

        // Act
        Action act = () => order.MarkAsPreparing();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("The order can only move to 'Preparing' after payment confirmation.");
    }

    [Fact(DisplayName = "Should mark order as Shipped")]
    public void Should_Mark_As_Shipped()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        SetOrderStatus(order, OrderStatus.Preparing);

        // Act
        order.MarkAsShipped();

        // Assert
        order.Status.Should().Be(OrderStatus.Shipped);
    }

    [Fact(DisplayName = "Should not mark as Shipped if not Preparing")]
    public void Should_Not_Mark_As_Shipped_If_Not_Preparing()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        SetOrderStatus(order, OrderStatus.PaymentConfirmed);

        // Act
        Action act = () => order.MarkAsShipped();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("The order can only be 'Shipped' after being 'Preparing'.");
    }

    [Fact(DisplayName = "Should mark order as Delivered")]
    public void Should_Mark_As_Delivered()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        SetOrderStatus(order, OrderStatus.Shipped);

        // Act
        order.MarkAsDelivered();

        // Assert
        order.Status.Should().Be(OrderStatus.Delivered);
    }

    [Fact(DisplayName = "Should not mark as Delivered if not Shipped")]
    public void Should_Not_Mark_As_Delivered_If_Not_Shipped()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        SetOrderStatus(order, OrderStatus.Preparing);

        // Act
        Action act = () => order.MarkAsDelivered();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("The order can only be marked as 'Delivered' after being 'Shipped'.");
    }

    // Cancellation

    [Fact(DisplayName = "Should cancel Pending order")]
    public void Should_Cancel_Pending_Order()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        order.AddItem(ValidProductId, "Produto", 50m, 1);

        // Act
        order.CancelOrder();

        // Assert
        order.Status.Should().Be(OrderStatus.Canceled);
    }

    [Fact(DisplayName = "Should cancel PaymentConfirmed order")]
    public void Should_Cancel_PaymentConfirmed_Order()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        order.AddItem(ValidProductId, "Produto", 50m, 1);
        var payment = order.StartPayment(PaymentMethod.Pix);
        order.HandlePaymentApproved(payment.Id); // Status: PaymentConfirmed

        // Act
        order.CancelOrder();

        // Assert
        order.Status.Should().Be(OrderStatus.Canceled);
    }

    [Theory(DisplayName = "Should not allow canceling order after Preparing")]
    [InlineData(OrderStatus.Preparing)]
    [InlineData(OrderStatus.Shipped)]
    [InlineData(OrderStatus.Delivered)]
    public void Should_Not_Cancel_After_Preparing(OrderStatus status)
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        SetOrderStatus(order, status);

        // Act
        Action act = () => order.CancelOrder();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot cancel an order that is already in preparation or beyond.");
    }
}