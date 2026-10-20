namespace Vendas.Application.Catalog.Commands.Categories.RenameCategory;

public sealed record RenameCategoryResultDto(
    Guid CategoryId,
    string CurrentName);