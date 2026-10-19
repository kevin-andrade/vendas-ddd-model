namespace Vendas.Application.Orders.Commands.UpdateShippingAddress;

public sealed record UpdateShippingAddressResultDto(
    Guid OrderId,
    string ShippingAddress,
    string Status);