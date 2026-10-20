using MediatR;

namespace Vendas.Application.Catalog.Commands.Categories.RenameCategory;

public sealed record RenameCategoryCommand(
    Guid CategoryId,
    string NewName) : IRequest<RenameCategoryResultDto>;