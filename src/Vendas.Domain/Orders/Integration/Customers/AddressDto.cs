namespace Vendas.Domain.Orders;

public sealed record AddressDto(
    Guid Id,
    string ZipCode,
    string Street,
    string Number,
    string Neighborhood,
    string City,
    string State,
    string Country,
    string Complement)
{
    public string Complement { get; init; } = Complement ?? string.Empty;
}