namespace Vendas.Application.Catalog.Commands.Categories.DeactivateCategory;

public sealed class DeactivateCategoryResultDto(
    Guid CategoryId,
    string Status);