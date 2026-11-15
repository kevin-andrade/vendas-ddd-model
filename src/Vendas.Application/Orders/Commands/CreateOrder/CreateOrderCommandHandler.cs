using MediatR;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;
using Vendas.Domain.Orders;
using Vendas.Domain.Orders.Integration.Customers;

namespace Vendas.Application.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResultDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomersGateway _customersGateway;
    private readonly CustomersAcl _customersAcl;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        ICustomersGateway customersGateway,
        CustomersAcl customersAcl)
    {
        _orderRepository = orderRepository;
        _customersGateway = customersGateway;
        _customersAcl = customersAcl;
    }

    public async Task<CreateOrderResultDto> Handle(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        // 1. Fetch address directly from Customers BC (Upstream)
        var addressDto = await _customersGateway.GetAddressAsync(
            command.CustomerId,
            command.AddressId,
            cancellationToken);

        Guard.Against<DomainException>(addressDto is null, "Address was not found.");

        // 2. ACL translates to internal model (Snapshot)
        var shippingAddress = _customersAcl.MapAddress(addressDto!);

        // 3. Create aggregate
        var order = Order.Create(command.CustomerId, shippingAddress);

        // 4. Persist
        await _orderRepository.AddAsync(order, cancellationToken);

        // 5. Return result
        return new CreateOrderResultDto(
            order.Id,
            order.OrderNumber,
            order.CreatedAt,
            order.TotalAmount,
            order.Status.ToString()
        );
    }
}