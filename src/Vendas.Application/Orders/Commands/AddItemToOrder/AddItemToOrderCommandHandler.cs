using MediatR;
using Vendas.Application.Abstractions.Persistence;

namespace Vendas.Application.Orders.Commands.AddItemToOrder;

public sealed class AddItemToOrderCommandHandler : IRequestHandler<AddItemToOrderCommand, AddItemToOrderResultDto>
{
    private readonly IOrderRepository _orderRepository;

    public AddItemToOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<AddItemToOrderResultDto> Handle(
        AddItemToOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository
            .GetByIdAsync(command.OrderId, cancellationToken);

        if (order is null)
            throw new InvalidOperationException("Order was not found.");
        
        order.AddItem(
            command.ProductId,
            command.ProductName,
            command.UnitPrice,
            command.Quantity
        );
        
        await _orderRepository.UpdateAsync(order, cancellationToken);
        
        return new AddItemToOrderResultDto(
            order.Id,
            order.TotalAmount,
            order.Status.ToString()
        );
    }
}