using MediatR;

namespace Vendas.Application.Catalog.Commands.Products.UpdateProductPrice;

public sealed record UpdateProductPriceCommand(
    Guid ProductId,
    decimal ProductPrice): IRequest<UpdateProductPriceResultDto>;