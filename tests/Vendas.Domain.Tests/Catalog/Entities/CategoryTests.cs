using FluentAssertions;
using Vendas.Domain.Catalog;
using Vendas.Domain.Catalog.Events;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Domain.Tests.Catalog.Entities;

public class CategoryTests
{

    [Fact]
    public void CreateCategory_ShouldCreateActiveWithValidName()
    {
        // Arrange
        var name = "Electronics";

        // Act
        var category = new Category(name);

        // Assert
        category.Name.Should().Be(name);
        category.IsActive.Should().BeTrue();
        category.CreatedAt.Should().NotBe(default);
        category.Description.Should().BeNull();
        category.DomainEvents.Should().BeEmpty(); // no event is raised in constructor
    }

    [Fact]
    public void CreateCategory_WithInvalidName_ShouldThrowDomainException()
    {
        // Arrange
        Action act = () => new Category("ab");

        // Assert
        act.Should()
           .Throw<DomainException>()
           .WithMessage("Name must be at least 3 characters long.");
    }

    [Fact]
    public void CreateCategory_WithEmptyName_ShouldThrowDomainException()
    {
        Action act = () => new Category("");

        act.Should()
           .Throw<DomainException>()
           .WithMessage("Name is required.");
    }

    [Fact]
    public void UpdateName_ShouldUpdateNameAndUpdatedAt()
    {
        var category = new Category("Accessories");

        category.UpdateName("Peripherals");

        category.Name.Should().Be("Peripherals");
        category.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void UpdateName_WithInvalidName_ShouldThrowDomainException()
    {
        var category = new Category("Accessories");

        Action act = () => category.UpdateName("ab");

        act.Should()
            .Throw<DomainException>()
            .WithMessage("Name must be at least 3 characters long.");
    }

    [Fact]
    public void UpdateDescription_ShouldUpdateDescriptionAndUpdatedAt()
    {
        var category = new Category("Computers");

        category.UpdateDescription("All about tech");

        category.Description.Should().Be("All about tech");
        category.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Activate_ShouldRaiseCategoryActivatedEvent()
    {
        // Arrange
        var category = new Category("Games");
        category.Deactivate();
        category.ClearDomainEvents(); // clear previous events

        // Act
        category.Activate();
        var events = category.DomainEvents;

        // Assert
        events.Should().ContainSingle()
            .Which.Should().BeOfType<CategoryActivatedEvent>();

        category.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Activate_WhenAlreadyActive_ShouldThrowDomainException()
    {
        var category = new Category("Clothing");

        Action act = () => category.Activate();

        act.Should()
           .Throw<DomainException>()
           .WithMessage("Category is already active.");
    }

    [Fact]
    public void Deactivate_ShouldRaiseCategoryDeactivatedEvent()
    {
        var category = new Category("Books");

        category.Deactivate();
        var events = category.DomainEvents;

        events.Should().ContainSingle()
              .Which.Should().BeOfType<CategoryInactivatedEvent>();

        category.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_ShouldThrowDomainException()
    {
        var category = new Category("Appliances");
        category.Deactivate();

        Action act = () => category.Deactivate();

        act.Should()
           .Throw<DomainException>()
           .WithMessage("Category is already inactive.");
    }

    [Fact]
    public void DomainEvents_ShouldBeAbleToClearEvents()
    {
        var category = new Category("Toys");
        category.Deactivate();

        category.DomainEvents.Should().HaveCount(1);

        category.ClearDomainEvents();

        category.DomainEvents.Should().BeEmpty();
    }
}