namespace Vendas.Application.Catalog.Commands.Products.UpdateProductCategory;

public sealed record UpdateProductCategoryResultDto(
    Guid ProductId,
    Guid NewCategoryId);