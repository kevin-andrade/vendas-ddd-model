using Vendas.Application.Abstractions.Persistence;
using Vendas.Application.Catalog.Commands.Categories.ActivateCategory;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Application.Catalog.Commands.Categories.DeactivateCategory;

public sealed class DeactivateCategoryCommandHandler
{
    private readonly ICategoryRepository  _categoryRepository;

    public DeactivateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    
    public async Task<DeactivateCategoryResultDto> Handle(ActivateCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken)
                       ?? throw new DomainException("Category was not found");
        
        category.Deactivate();

        await _categoryRepository.UpdateAsync(category, cancellationToken);

        return new DeactivateCategoryResultDto(category.Id,  category.IsActive.ToString());
    }
}