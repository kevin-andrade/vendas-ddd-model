using MediatR;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Application.Catalog.Commands.Categories.ActivateCategory;

public sealed class ActivateCategoryCommandHandler : IRequestHandler<ActivateCategoryCommand, ActivateCategoryResultDto>
{
    private readonly ICategoryRepository  _categoryRepository;

    public ActivateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    
    public async Task<ActivateCategoryResultDto> Handle(ActivateCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken)
            ?? throw new DomainException("Category was not found");
        
        category.Activate();

        await _categoryRepository.UpdateAsync(category, cancellationToken);

        return new ActivateCategoryResultDto(category.Id,  category.IsActive.ToString());
    }
}