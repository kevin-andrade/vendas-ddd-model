using Vendas.Domain.Catalog.Enums;
using Vendas.Domain.Catalog.Events;
using Vendas.Domain.Catalog.ValueObjects;
using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Catalog;

public sealed class Product : AggregateRoot
{
    public ProductName Name { get; private set; }
    public ProductCode Code { get; private set; }
    public ProductPrice Price { get; private set; }
    public string? Description { get; private set; }
    public Guid CategoryId { get; private set; }
    public ProductStatus Status { get; private set; }
    public int Stock { get; private set; }

    private readonly List<ProductImage> _images = new();
    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

    public Product(
        ProductName name,
        ProductCode code,
        ProductPrice price,
        Guid categoryId,
        int initialStock = 0,
        string? description = null)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(code, nameof(code));
        Guard.AgainstNull(price, nameof(price));
        Guard.AgainstEmptyGuid(categoryId, nameof(categoryId));
        Guard.Against<DomainException>(initialStock < 0,
            "Initial stock cannot be negative.");

        Name = name;
        Code = code;
        Price = price;
        CategoryId = categoryId;
        Description = description?.Trim();
        Stock = initialStock;

        Status = ProductStatus.Active;
    }

    public void UpdateName(ProductName newName)
    {
        Guard.AgainstNull(newName, nameof(newName));
        Name = newName;
        SetUpdatedAt();
    }

    public void UpdatePrice(ProductPrice newPrice)
    {
        Guard.AgainstNull(newPrice, nameof(newPrice));

        var oldPrice = Price.Value;
        var currentPrice = newPrice.Value;

        Price = newPrice;
        SetUpdatedAt();

        AddDomainEvent(new ProductPriceChangedEvent(Id, oldPrice, currentPrice));
    }

    public void UpdateCategory(Guid newCategoryId)
    {
        Guard.AgainstEmptyGuid(newCategoryId, nameof(newCategoryId));
        CategoryId = newCategoryId;
        SetUpdatedAt();
    }

    public void UpdateDescription(string? newDescription)
    {
        Description = newDescription?.Trim();
        SetUpdatedAt();
    }

    public void AdjustStock(int quantity, string reason)
    {
        Guard.AgainstNullOrEmptyString(reason, nameof(reason));
        Guard.Against<DomainException>(Stock + quantity < 0,
            "Stock adjustment would result in a negative balance.");

        Stock += quantity;
        SetUpdatedAt();

        AddDomainEvent(new StockAdjustedEvent(Id, quantity, reason));
    }

    public void Activate()
    {
        Guard.Against<DomainException>(Status == ProductStatus.Active,
            "Product is already active.");

        Status = ProductStatus.Active;
        SetUpdatedAt();

        AddDomainEvent(new ProductActivatedEvent(Id));
    }

    public void Deactivate()
    {
        Guard.Against<DomainException>(Status == ProductStatus.Inactive,
            "Product is already inactive.");

        Status = ProductStatus.Inactive;
        SetUpdatedAt();

        AddDomainEvent(new ProductInactivatedEvent(Id));
    }

    public void AddImage(ProductImage image)
    {
        Guard.AgainstNull(image, nameof(image));

        Guard.Against<DomainException>(
            _images.Any(i => i.Order == image.Order),
            "An image with this order already exists.");

        _images.Add(image);

        SetUpdatedAt();
        AddDomainEvent(new ImageAddedEvent(Id, image.Url, image.Order));
    }
}