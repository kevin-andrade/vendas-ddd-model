using MediatR;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;
using Vendas.Domain.Orders;

namespace Vendas.Application.Orders.Commands.AddItemToOrder;

public sealed class AddItemToOrderCommandHandler : IRequestHandler<AddItemToOrderCommand, AddItemToOrderResultDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICatalogGateway _catalogGateway;
    private readonly CatalogAcl _catalogAcl;

    public AddItemToOrderCommandHandler(
        IOrderRepository orderRepository,
        ICatalogGateway catalogGateway,
        CatalogAcl catalogAcl)
    {
        _orderRepository = orderRepository;
        _catalogGateway = catalogGateway;
        _catalogAcl = catalogAcl;
    }

    public async Task<AddItemToOrderResultDto> Handle(
        AddItemToOrderCommand request, 
        CancellationToken cancellationToken)
    {
        // 1. Fetch the order aggregate
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        Guard.Against<DomainException>(order is null, "Order was not found.");

        // 2. Fetch product info from Catalog BC (Upstream)
        var productDto = await _catalogGateway.GetProductByIdAsync(request.ProductId, cancellationToken);
        Guard.Against<DomainException>(productDto is null, "Product was not found.");

        // 3. ACL maps to internal model/values
        var (productName, unitPrice) = _catalogAcl.MapProduct(productDto!);

        // 4. Execute domain rule on aggregate
        order!.AddItem(request.ProductId, productName, unitPrice, request.Quantity);

        // 5. Persist state
        await _orderRepository.UpdateAsync(order, cancellationToken);

        // 6. Return DTO result
        return new AddItemToOrderResultDto(
            order.Id,
            order.TotalAmount,
            order.Status.ToString()
        );
    }
}