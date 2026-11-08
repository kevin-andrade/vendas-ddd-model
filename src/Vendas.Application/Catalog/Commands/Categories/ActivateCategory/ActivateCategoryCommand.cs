using MediatR;

namespace Vendas.Application.Catalog.Commands.Categories.ActivateCategory;

public sealed record ActivateCategoryCommand(
    Guid CategoryId) : IRequest<ActivateCategoryResultDto>;