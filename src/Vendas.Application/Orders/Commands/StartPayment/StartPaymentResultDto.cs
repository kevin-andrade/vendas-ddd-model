namespace Vendas.Application.Orders.Commands.StartPayment;

public sealed record StartPaymentResultDto(
    Guid OrderId,
    Guid PaymentId,
    string StatusOrder,
    string StatusPayment);