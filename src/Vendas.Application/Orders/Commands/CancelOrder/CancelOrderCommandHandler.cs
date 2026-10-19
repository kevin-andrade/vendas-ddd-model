using MediatR;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Orders;

namespace Vendas.Application.Orders.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, CancelOrderResultDto>
{
    private readonly IOrderRepository _orderRepository;
    
    public CancelOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    public async Task<CancelOrderResultDto> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken)
            ?? throw new DomainException("Order was not found");

        var reason = new CancellationReason(command.ReasonCode);
        
        order.CancelOrder(reason);

        await _orderRepository.UpdateAsync(order, cancellationToken);

        return new CancelOrderResultDto(
            order.Id,
            order.Status.ToString());
    }
}