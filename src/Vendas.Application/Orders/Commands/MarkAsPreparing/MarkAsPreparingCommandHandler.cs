using MediatR;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Application.Orders.Commands.MarkAsPreparing;

public sealed class MarkAsPreparingCommandHandler : IRequestHandler<MarkAsPreparingCommand, MarkAsPreparingResultDto>
{
    private readonly IOrderRepository _orderRepository;

    public MarkAsPreparingCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    public async Task<MarkAsPreparingResultDto> Handle(MarkAsPreparingCommand command, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken)
            ?? throw new DomainException("Order was not found");
        
        order.MarkAsPreparing();
        
        await _orderRepository.UpdateAsync(order, cancellationToken);

        return new MarkAsPreparingResultDto(
            order.Id,
            order.Status.ToString());
    }
}