using MediatR;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Application.Catalog.Commands.Products.AdjustStock;

public sealed class AdjustStockCommandHandler : IRequestHandler<AdjustStockCommand, AdjustStockResultDto>
{
    private readonly IProductRepository _productRepository;
    
    public AdjustStockCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    public async Task<AdjustStockResultDto> Handle(AdjustStockCommand command, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(command.ProductId, cancellationToken)
            ?? throw new DomainException("Product was not found");
        
        product.AdjustStock(command.Quantity, command.Reason);

        await _productRepository.UpdateAsync(product, cancellationToken);

        return new AdjustStockResultDto(product.Id, product.Stock);
    }
}