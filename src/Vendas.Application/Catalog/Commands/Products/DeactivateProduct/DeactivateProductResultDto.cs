namespace Vendas.Application.Catalog.Commands.Products.DeactivateProduct;

public sealed record DeactivateProductResultDto(
    Guid ProductId,
    string Status);