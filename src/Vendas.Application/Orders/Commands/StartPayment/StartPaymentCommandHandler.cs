using MediatR;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Application.Orders.Commands.StartPayment;

public sealed class StartPaymentCommandHandler : IRequestHandler<StartPaymentCommand, StartPaymentResultDto>
{
    private readonly IOrderRepository _orderRepository;

    public StartPaymentCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    public async Task<StartPaymentResultDto> Handle(StartPaymentCommand command, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken)
            ?? throw new DomainException("Order was not found");

        var payment = order.StartPayment(command.PaymentMethod);

        await _orderRepository.UpdateAsync(order, cancellationToken);

        return new StartPaymentResultDto(
            order.Id,
            payment.Id,
            order.Status.ToString(),
            payment.PaymentStatus.ToString()
        );
    }
}