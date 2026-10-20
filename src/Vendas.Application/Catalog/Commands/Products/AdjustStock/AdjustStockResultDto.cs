namespace Vendas.Application.Catalog.Commands.Products.AdjustStock;

public sealed record AdjustStockResultDto(
    Guid ProductId,
    int CurrentStock);