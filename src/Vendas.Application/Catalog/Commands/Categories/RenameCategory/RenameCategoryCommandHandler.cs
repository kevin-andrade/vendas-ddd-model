using MediatR;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Application.Catalog.Commands.Categories.RenameCategory;

public sealed class RenameCategoryCommandHandler : IRequestHandler<RenameCategoryCommand, RenameCategoryResultDto>
{
    private readonly ICategoryRepository _categoryRepository;

    public RenameCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    
    public async Task<RenameCategoryResultDto> Handle(RenameCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken)
            ?? throw new DomainException("Category was not found");
        
        category.UpdateName(command.NewName);

        await _categoryRepository.UpdateAsync(category, cancellationToken);
        
        return new RenameCategoryResultDto(category.Id, category.Name);
    }
}