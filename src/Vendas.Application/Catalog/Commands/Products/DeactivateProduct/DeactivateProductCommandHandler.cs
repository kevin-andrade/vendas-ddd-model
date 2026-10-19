using MediatR;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Application.Catalog.Commands.Products.DeactivateProduct;

public sealed class DeactivateProductCommandHandler : IRequestHandler<DeactivateProductCommand, DeactivateProductResultDto>
{
    private readonly IProductRepository _productRepository;

    public DeactivateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    public async Task<DeactivateProductResultDto> Handle(DeactivateProductCommand command, CancellationToken cancellationToken)
    {
       var product = await _productRepository.GetByIdAsync(command.ProductId, cancellationToken)
           ?? throw new DomainException("Product was not found");
       
       product.Deactivate();
       
       await _productRepository.UpdateAsync(product, cancellationToken);
       
       return new DeactivateProductResultDto(product.Id, product.Status.ToString());
    }
}