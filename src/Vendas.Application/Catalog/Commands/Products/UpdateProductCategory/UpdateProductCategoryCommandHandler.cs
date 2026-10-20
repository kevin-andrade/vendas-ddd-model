using MediatR;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Application.Catalog.Commands.Products.UpdateProductCategory;

public sealed class UpdateProductCategoryCommandHandler : IRequestHandler<UpdateProductCategoryCommand, UpdateProductCategoryResultDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    
    public async Task<UpdateProductCategoryResultDto> Handle(UpdateProductCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(command.NewCategoryId, cancellationToken)
            ?? throw new DomainException("Category was not found");
        
        Guard.Against<DomainException>(
            !category.IsActive,
            "It is not possible to associate a product with an inactive category.");
        
        var product = await _productRepository.GetByIdAsync(command.ProductId, cancellationToken)
            ?? throw new DomainException("Product was not found");
        
        product.UpdateCategory(command.NewCategoryId);

        await _productRepository.UpdateAsync(product, cancellationToken);
        
        return new UpdateProductCategoryResultDto(product.Id, product.CategoryId);
    }
}