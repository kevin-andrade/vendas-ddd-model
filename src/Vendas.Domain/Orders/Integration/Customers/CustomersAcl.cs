using Vendas.Domain.Orders.ValueObjects;

namespace Vendas.Domain.Orders.Integration.Customers;

public sealed class CustomersAcl
{
    public ShippingAddress MapAddress(AddressDto dto)
    {
        return ShippingAddress.Create(
            dto.ZipCode,
            dto.Street,
            dto.Number,
            dto.Neighborhood,
            dto.City,
            dto.State,
            dto.Country,
            dto.Complement
        );
    }
}