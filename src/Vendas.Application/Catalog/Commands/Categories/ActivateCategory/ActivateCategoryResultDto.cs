namespace Vendas.Application.Catalog.Commands.Categories.ActivateCategory;

public sealed record ActivateCategoryResultDto(
    Guid CategoryId, 
    string Status);