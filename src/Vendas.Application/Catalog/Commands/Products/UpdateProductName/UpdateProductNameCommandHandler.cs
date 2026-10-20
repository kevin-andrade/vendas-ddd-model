using MediatR;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Catalog.ValueObjects;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Application.Catalog.Commands.Products.UpdateProductName;

public sealed class UpdateProductNameCommandHandler : IRequestHandler<UpdateProductNameCommand, UpdateProductNameResultDto>
{
    private readonly IProductRepository _productRepository;
    
    public UpdateProductNameCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    public async Task<UpdateProductNameResultDto> Handle(UpdateProductNameCommand command, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(command.ProductId, cancellationToken)
                      ?? throw new DomainException("Product was not found");
        
        product.UpdateName(new ProductName(command.ProductName));

        await _productRepository.UpdateAsync(product, cancellationToken);
        
        return new UpdateProductNameResultDto(product.Id, product.Name.Value);
    }
}