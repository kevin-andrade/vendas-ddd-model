namespace Vendas.Application.Catalog.Commands.Products.UpdateProductName;

public sealed record UpdateProductNameResultDto(
    Guid ProductId,
    string ProductName);