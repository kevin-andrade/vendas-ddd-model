using MediatR;

namespace Vendas.Application.Catalog.Commands.Products.UpdateProductCategory;

public sealed record UpdateProductCategoryCommand(
    Guid ProductId,
    Guid NewCategoryId) : IRequest<UpdateProductCategoryResultDto>;