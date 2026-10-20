using MediatR;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Application.Catalog.Commands.Products.ActivateProduct;

public sealed class ActivateProductCommandHandler : IRequestHandler<ActivateProductCommand, ActivateProductResultDto>
{
    private readonly IProductRepository _productRepository;

    public ActivateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ActivateProductResultDto> Handle(ActivateProductCommand command,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(command.ProductId, cancellationToken) 
                      ?? throw new DomainException("Product was not found");
        
        product.Activate();

        await _productRepository.UpdateAsync(product, cancellationToken);

        return new ActivateProductResultDto(product.Id, product.Status.ToString());
    }
}