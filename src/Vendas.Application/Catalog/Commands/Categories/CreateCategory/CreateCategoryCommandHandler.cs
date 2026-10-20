using MediatR;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Catalog;

namespace Vendas.Application.Catalog.Commands.Categories.CreateCategory;

public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CreateCategoryResultDto>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    
    public async Task<CreateCategoryResultDto> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = new Category(command.Name, command.Description);

        await _categoryRepository.AddAsync(category, cancellationToken);
        
        return new CreateCategoryResultDto(category.Id, category.Name, category.IsActive);
    }
}