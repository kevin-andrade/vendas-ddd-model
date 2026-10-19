using MediatR;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Application.Orders.Commands.MarkAsShipped;

public sealed class MarkAsShippedCommandHandler : IRequestHandler<MarkAsShippedCommand, MarkAsShippedResultDto>
{
    private readonly IOrderRepository _orderRepository;

    public MarkAsShippedCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    public async Task<MarkAsShippedResultDto> Handle(MarkAsShippedCommand command, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId,  cancellationToken)
            ?? throw new DomainException("Order was not found");
        
        order.MarkAsShipped();

        await _orderRepository.UpdateAsync(order, cancellationToken);
        
        return new MarkAsShippedResultDto(order.Id, order.Status.ToString());
    }
}