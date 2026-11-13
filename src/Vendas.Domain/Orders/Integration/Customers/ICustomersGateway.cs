namespace Vendas.Domain.Orders.Integration.Customers;

public interface ICustomersGateway
{
    Task<AddressDto?> GetAddressAsync(
        Guid customerId,
        Guid addressId,
        CancellationToken cancellationToken);
}