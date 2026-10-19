using MediatR;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Catalog.ValueObjects;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Application.Catalog.Commands.Products.UpdateProductPrice;

public sealed class UpdateProductPriceCommandHandler : IRequestHandler<UpdateProductPriceCommand, UpdateProductPriceResultDto>
{
    private readonly IProductRepository _productRepository;
    
    public UpdateProductPriceCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    public async Task<UpdateProductPriceResultDto> Handle(UpdateProductPriceCommand command, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(command.ProductId, cancellationToken)
                      ?? throw new DomainException("Product was not found.");
        
        product.UpdatePrice(new ProductPrice(command.ProductPrice));

        await _productRepository.UpdateAsync(product, cancellationToken);
        
        return new UpdateProductPriceResultDto(product.Id, product.Price.Value);
    }
}