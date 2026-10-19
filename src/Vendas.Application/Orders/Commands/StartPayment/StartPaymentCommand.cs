using MediatR;
using Vendas.Domain.Orders.Enums;

namespace Vendas.Application.Orders.Commands.StartPayment;

public sealed record StartPaymentCommand(
    Guid OrderId,
    PaymentMethod  PaymentMethod) : IRequest<StartPaymentResultDto>;