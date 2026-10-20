using MediatR;
using Vendas.Application.Catalog.Commands.Products.ActivateProduct;

namespace Vendas.Application.Catalog.Commands.Categories.ActivateCategory;

public sealed record ActivateCategoryCommand(
    Guid CategoryId) : IRequest<ActivateCategoryResultDto>;