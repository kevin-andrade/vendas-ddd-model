using MediatR;

namespace Vendas.Application.Catalog.Commands.Products.AdjustStock;

public sealed record AdjustStockCommand(
    Guid ProductId,
    int Quantity,
    string Reason) : IRequest<AdjustStockResultDto>;