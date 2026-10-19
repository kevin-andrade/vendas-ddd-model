using MediatR;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Orders;

namespace Vendas.Application.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResultDto>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<CreateOrderResultDto> Handle(
        CreateOrderCommand command, 
        CancellationToken cancellationToken)
    {
        var order = Order.Create(
            command.CustomerId,
            command.DeliveryAddress
        );

        await _orderRepository.AddAsync(order, cancellationToken);

        return new CreateOrderResultDto(
            order.Id,
            order.OrderNumber,
            order.CreatedAt,
            order.TotalAmount,
            order.Status.ToString()
        );
    }
}