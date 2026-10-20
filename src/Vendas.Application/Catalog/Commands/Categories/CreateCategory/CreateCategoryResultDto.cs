namespace Vendas.Application.Catalog.Commands.Categories.CreateCategory;

public sealed record CreateCategoryResultDto(
    Guid CategoryId,
    string Name,
    bool IsActive);