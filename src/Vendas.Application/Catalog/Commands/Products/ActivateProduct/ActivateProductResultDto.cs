namespace Vendas.Application.Catalog.Commands.Products.ActivateProduct;

public sealed record ActivateProductResultDto(
    Guid ProductId,
    string Status);