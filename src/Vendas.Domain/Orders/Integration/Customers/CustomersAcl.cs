using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Orders.Integration.Customers;

public sealed class CustomersAcl
{
    private readonly ICustomersGateway _gateway;

    public CustomersAcl(ICustomersGateway gateway)
    {
        Guard.AgainstNull(gateway, nameof(gateway));
        _gateway = gateway;
    }

    public async Task<ShippingAddressSnapshot> GetShippingAddressSnapshotAsync(
        Guid customerId, Guid addressId, CancellationToken cancellationToken = default)
    {
        var dto = await _gateway.GetAddressAsync(
            customerId,
            addressId,
            cancellationToken);

        if (dto is null)
            throw new DomainException("Address not found in Customers context.");

        return new ShippingAddressSnapshot(dto.ZipCode, dto.Street, dto.Number, dto.Neighborhood,
            dto.City, dto.State, dto.Country, dto.Complement);
    }
}