using FluentAssertions;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Customers;

namespace Vendas.Domain.Tests.Customers.Entities;

public class AddressTests
{
    private static Address CreateValidAddress()
    {
        return new Address(
            zipCode: "12345678",
            street: "Rua A",
            number: "100",
            neighborhood: "Centro",
            city: "São Paulo",
            state: "SP",
            country: "Brasil"
        );
    }

    [Fact]
    public void Should_Create_Valid_Address()
    {
        // Arrange & Act
        var address = CreateValidAddress();

        // Assert
        address.ZipCode.Should().Be("12345678");
        address.Street.Should().Be("Rua A");
        address.Number.Should().Be("100");
        address.Neighborhood.Should().Be("Centro");
        address.City.Should().Be("São Paulo");
        address.State.Should().Be("SP");
        address.Country.Should().Be("Brasil");
        address.Complement.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Throw_Error_When_ZipCode_Is_Invalid(string? invalidZipCode)
    {
        // Arrange
        Action act = () => new Address(
            zipCode: invalidZipCode!,
            street: "Rua A",
            number: "100",
            neighborhood: "Centro",
            city: "São Paulo",
            state: "SP",
            country: "Brasil"
        );

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Zip code is required.");
    }

    [Fact]
    public void Should_Throw_Error_When_ZipCode_Does_Not_Have_8_Digits()
    {
        // Arrange
        Action act = () => new Address(
            zipCode: "1234",
            street: "Rua A",
            number: "100",
            neighborhood: "Centro",
            city: "São Paulo",
            state: "SP",
            country: "Brasil"
        );

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Invalid zip code.");
    }

    [Theory]
    [InlineData(null, "100", "Centro", "São Paulo", "SP", "Brasil")]
    [InlineData("Rua A", null, "Centro", "São Paulo", "SP", "Brasil")]
    [InlineData("Rua A", "100", null, "São Paulo", "SP", "Brasil")]
    [InlineData("Rua A", "100", "Centro", null, "SP", "Brasil")]
    [InlineData("Rua A", "100", "Centro", "São Paulo", null, "Brasil")]
    [InlineData("Rua A", "100", "Centro", "São Paulo", "SP", null)]
    public void Should_Throw_Error_When_Required_Fields_Are_Invalid(
        string? street,
        string? number,
        string? neighborhood,
        string? city,
        string? state,
        string? country)
    {
        // Arrange
        Action act = () => new Address(
            zipCode: "12345678",
            street: street!,
            number: number!,
            neighborhood: neighborhood!,
            city: city!,
            state: state!,
            country: country!
        );

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Should_Update_Address_With_Valid_Data()
    {
        // Arrange
        var address = CreateValidAddress();

        // Act
        address.Update(
            zipCode: "87654321",
            street: "Rua B",
            number: "200",
            neighborhood: "Bairro Novo",
            city: "Rio de Janeiro",
            state: "RJ",
            country: "Brasil",
            complement: "Apto 12"
        );

        // Assert
        address.ZipCode.Should().Be("87654321");
        address.Street.Should().Be("Rua B");
        address.Number.Should().Be("200");
        address.Neighborhood.Should().Be("Bairro Novo");
        address.City.Should().Be("Rio de Janeiro");
        address.State.Should().Be("RJ");
        address.Country.Should().Be("Brasil");
        address.Complement.Should().Be("Apto 12");
    }

    [Fact]
    public void Should_Throw_Error_On_Update_With_Invalid_ZipCode()
    {
        // Arrange
        var address = CreateValidAddress();

        // Act
        Action act = () => address.Update(
            zipCode: "123",
            street: "Rua Teste",
            number: "10",
            neighborhood: "Centro",
            city: "SP",
            state: "SP",
            country: "Brasil"
        );

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Invalid zip code.");
    }

    [Fact]
    public void Should_Throw_Error_On_Update_With_Invalid_Required_Field()
    {
        // Arrange
        var address = CreateValidAddress();

        // Act
        Action act = () => address.Update(
            zipCode: "12345678",
            street: "",
            number: "10",
            neighborhood: "Centro",
            city: "SP",
            state: "SP",
            country: "Brasil"
        );

        // Assert
        act.Should().Throw<DomainException>();
    }
}