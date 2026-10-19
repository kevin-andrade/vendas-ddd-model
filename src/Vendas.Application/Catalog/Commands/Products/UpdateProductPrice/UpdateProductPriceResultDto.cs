namespace Vendas.Application.Catalog.Commands.Products.UpdateProductPrice;

public sealed record UpdateProductPriceResultDto(
    Guid ProductId,
    decimal ProductPrice);
