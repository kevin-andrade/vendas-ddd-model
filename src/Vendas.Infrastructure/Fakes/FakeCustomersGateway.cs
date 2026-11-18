using Vendas.Domain.Orders;
using Vendas.Domain.Orders.Integration.Customers;

namespace Vendas.Infrastructure.Fakes;

public sealed class FakeCustomersGateway : ICustomersGateway
{
    private static readonly Dictionary<Guid, Dictionary<Guid, AddressDto>> _customers = new()
    {
        [new Guid("22222222-0000-0000-0000-000000000001")] = new()
        {
            [new Guid("33333333-0000-0000-0000-000000000001")] = new(
                Id: new Guid("33333333-0000-0000-0000-000000000001"),
                ZipCode: "01310-100",
                Street: "Avenida Paulista",
                Number: "1578",
                Neighborhood: "Bela Vista",
                City: "São Paulo",
                State: "SP",
                Country: "Brazil",
                Complement: "Suite 42"
            ),
        },
    };

    public Task<AddressDto?> GetAddressAsync(Guid customerId, Guid addressId, CancellationToken cancellationToken = default)
    {
        if (_customers.TryGetValue(customerId, out var addresses) 
            && addresses.TryGetValue(addressId, out var address))
        {
            return Task.FromResult<AddressDto?>(address);
        }

        return Task.FromResult<AddressDto?>(null);
    }
}