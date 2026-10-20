using MediatR;

namespace Vendas.Application.Catalog.Commands.Categories.DeactivateCategory;

public sealed record DeactivateCategoryCommand(Guid CategoryId) : IRequest<DeactivateCategoryResultDto>;