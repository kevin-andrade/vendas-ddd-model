using MediatR;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Application.Orders.Commands.MarkAsDelivered;

public sealed class MarkAsDeliveredCommandHandler : IRequestHandler<MarkAsDeliveredCommand, MarkAsDeliveredResultDto>
{
    private readonly  IOrderRepository _orderRepository;

    public MarkAsDeliveredCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<MarkAsDeliveredResultDto> Handle(MarkAsDeliveredCommand command, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken)
                    ?? throw new DomainException("Order was not found");
        
        order.MarkAsDelivered();

        await _orderRepository.UpdateAsync(order, cancellationToken);
        
        return new MarkAsDeliveredResultDto(order.Id,  order.Status.ToString());
    }
}