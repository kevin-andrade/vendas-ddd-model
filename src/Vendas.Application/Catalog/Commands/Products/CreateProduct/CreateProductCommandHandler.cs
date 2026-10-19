using MediatR;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Catalog;
using Vendas.Domain.Catalog.ValueObjects;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Application.Catalog.Commands.Products.CreateProduct;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductResultDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CreateProductCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }
    
    public async Task<CreateProductResultDto> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken)
            ?? throw new DomainException("Category was not found");
        
        Guard.Against<DomainException>(
            !category.IsActive,
            "Cannot create a product in an inactive category.");
        
        var name = new ProductName(command.Name);
        var code = new ProductCode(command.Code);
        var price = new ProductPrice(command.Price);

        var product = new Product(
            name,
            code,
            price,
            command.CategoryId,
            command.InitialStock,
            command.Description);

        await _productRepository.AddAsync(product, cancellationToken);

        return new CreateProductResultDto(
            product.Id,
            product.Name.Value,
            product.Price.Value,
            product.Status.ToString());
    }
}