namespace Vendas.Application.Catalog.Commands.Products.CreateProduct;

public sealed record CreateProductResultDto(
    Guid ProductId,
    string ProductName,
    decimal ProductPrice,
    string Status);