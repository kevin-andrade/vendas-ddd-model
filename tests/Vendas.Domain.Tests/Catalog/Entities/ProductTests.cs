using FluentAssertions;
using Vendas.Domain.Catalog;
using Vendas.Domain.Catalog.Enums;
using Vendas.Domain.Catalog.Events;
using Vendas.Domain.Catalog.ValueObjects;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Domain.Tests.Catalog.Entities;

public class ProductTests
{
    // Helper method to create a valid Product
    private Product CreateProduct(
        string name = "Professional Camera",
        string code = "CAM-001",
        decimal price = 2500m,
        int stock = 10,
        string? description = null)
    {
        return new Product(
            new ProductName(name),
            new ProductCode(code),
            new ProductPrice(price),
            Guid.NewGuid(),
            stock,
            description
        );
    }

    [Fact]
    public void CreateProduct_ShouldBeCreatedAsActive()
    {
        var product = CreateProduct();

        product.Status.Should().Be(ProductStatus.Active);
    }

    [Fact]
    public void CreateProduct_WithNegativeStock_ShouldThrowDomainException()
    {
        Action act = () => CreateProduct(stock: -1);

        act.Should().Throw<DomainException>()
           .WithMessage("*Initial stock cannot be negative*");
    }

    [Fact]
    public void UpdateName_ShouldUpdateName()
    {
        var product = CreateProduct();

        product.UpdateName(new ProductName("Mirrorless Camera"));

        product.Name.Value.Should().Be("Mirrorless Camera");
    }

    [Fact]
    public void UpdatePrice_ShouldUpdatePriceAndRaiseEvent()
    {
        var product = CreateProduct();
        product.ClearDomainEvents();

        var newPrice = new ProductPrice(3000m);

        product.UpdatePrice(newPrice);

        product.Price.Value.Should().Be(3000m);

        product.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ProductPriceChangedEvent>();

        var evt = (ProductPriceChangedEvent)product.DomainEvents.Single();
        evt.OldPrice.Should().Be(2500m);
        evt.NewPrice.Should().Be(3000m);
    }

    [Fact]
    public void AdjustStock_ShouldChangeStockAndRaiseEvent()
    {
        var product = CreateProduct();
        product.ClearDomainEvents();

        product.AdjustStock(5, "Restock");

        product.Stock.Should().Be(15);

        product.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<StockAdjustedEvent>();
    }

    [Fact]
    public void AdjustStock_NegativeResult_ShouldThrowDomainException()
    {
        var product = CreateProduct(stock: 5);

        Action act = () => product.AdjustStock(-10, "Adjustment error");

        act.Should().Throw<DomainException>()
           .WithMessage("*Stock adjustment would result in a negative balance*");
    }

    [Fact]
    public void Deactivate_ShouldChangeStatusAndRaiseEvent()
    {
        var product = CreateProduct();
        product.ClearDomainEvents();

        product.Deactivate();

        product.Status.Should().Be(ProductStatus.Inactive);

        product.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ProductInactivatedEvent>();
    }

    [Fact]
    public void Activate_ShouldChangeStatusAndRaiseEvent()
    {
        var product = CreateProduct();

        product.Deactivate();
        product.ClearDomainEvents();

        product.Activate();

        product.Status.Should().Be(ProductStatus.Active);

        product.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ProductActivatedEvent>();
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_ShouldThrowDomainException()
    {
        var product = CreateProduct();
        product.Deactivate();

        Action act = () => product.Deactivate();

        act.Should().Throw<DomainException>()
           .WithMessage("*already inactive*");
    }

    [Fact]
    public void Activate_WhenAlreadyActive_ShouldThrowDomainException()
    {
        var product = CreateProduct();

        Action act = () => product.Activate();

        act.Should().Throw<DomainException>()
           .WithMessage("*already active*");
    }

    [Fact]
    public void UpdateDescription_ShouldUpdateDescription()
    {
        var product = CreateProduct();

        product.UpdateDescription("Updated Description");

        product.Description.Should().Be("Updated Description");
    }

    [Fact]
    public void AddImage_ShouldAddImageAndRaiseEvent()
    {
        var product = CreateProduct();
        product.ClearDomainEvents();

        var image = new ProductImage("http://img.com/1.jpg", 1);

        product.AddImage(image);

        product.Images.Should().HaveCount(1);

        product.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ImageAddedEvent>();
    }

    [Fact]
    public void AddImage_WithDuplicateDisplayOrder_ShouldThrowDomainException()
    {
        var product = CreateProduct();

        product.AddImage(new ProductImage("http://img.com/1.jpg", 1));

        Action act = () =>
            product.AddImage(new ProductImage("http://img.com/2.jpg", 1));

        act.Should().Throw<DomainException>()
           .WithMessage("*An image with this order already exists*");
    }
}