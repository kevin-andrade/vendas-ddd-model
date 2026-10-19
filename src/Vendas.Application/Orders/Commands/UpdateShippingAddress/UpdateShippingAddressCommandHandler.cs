using MediatR;
using Vendas.Application.Abstractions.Persistence;

namespace Vendas.Application.Orders.Commands.UpdateShippingAddress;

public sealed class UpdateShippingAddressCommandHandler : IRequestHandler<UpdateShippingAddressCommand, UpdateShippingAddressResultDto>
{
    private readonly IOrderRepository _orderRepository;
    
    public  UpdateShippingAddressCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    public async Task<UpdateShippingAddressResultDto> Handle(UpdateShippingAddressCommand command, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken);
        
        if (order is null)
            throw new InvalidOperationException("Order was not found.");
        
        order.UpdateShippingAddress(command.NewShippingAddress);

        await _orderRepository.UpdateAsync(order, cancellationToken);
        
        return new UpdateShippingAddressResultDto(
            order.Id,
            order.ShippingAddress.FormatAddress(),
            order.Status.ToString()
        );
    }
}