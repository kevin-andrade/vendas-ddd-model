using FluentAssertions;
using Vendas.Domain.Orders;

namespace Vendas.Domain.Tests.Orders.Entities;

public class OrderItemTests
{
    private static OrderItem CreateValidItem(decimal price = 100m,  int quantity = 2)
        => new OrderItem(Guid.NewGuid(), "Produto Teste",  price, quantity);

    [Fact(DisplayName = "Order Items Valid")]
    private void GivenValidData_WhenCreate_ThenShouldReturnOrderItem()
    {
        var productId = Guid.NewGuid();
        var productName = "Playstation 5";
        var productPrice = 500m;
        var productQuantity = 1;
        
        var item = new OrderItem(productId,  productName, productPrice, productQuantity);
        
        item.ProductId.Should().Be(productId);
        item.ProductName.Should().Be(productName);
        item.UnitPrice.Should().Be(productPrice);
        item.DiscountApplied.Should().Be(0);
        item.TotalPrice.Should().Be(500);
    }
}

