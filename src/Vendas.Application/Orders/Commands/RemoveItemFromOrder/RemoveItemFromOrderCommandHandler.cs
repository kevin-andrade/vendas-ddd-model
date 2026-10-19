using MediatR;
using Vendas.Application.Abstractions.Persistence;

namespace Vendas.Application.Orders.Commands.RemoveItemFromOrder;

public sealed class RemoveItemFromOrderCommandHandler : IRequestHandler<RemoveItemFromOrderCommand, RemoveItemFromOrderResultDto>
{
    private readonly IOrderRepository _orderRepository;

    public RemoveItemFromOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    public async Task<RemoveItemFromOrderResultDto> Handle(RemoveItemFromOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await _orderRepository
            .GetByIdAsync(command.OrderId, cancellationToken);

        if (order is null)
            throw new InvalidOperationException("Order was not found.");
        
        order.RemoveItem(command.ProductId);
        
        await _orderRepository.UpdateAsync(order, cancellationToken);
        
        return new RemoveItemFromOrderResultDto(
            order.Id,
            order.TotalAmount,
            order.Status.ToString()
        );
    }
}