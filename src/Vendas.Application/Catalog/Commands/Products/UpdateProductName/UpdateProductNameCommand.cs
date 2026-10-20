using MediatR;

namespace Vendas.Application.Catalog.Commands.Products.UpdateProductName;

public sealed record UpdateProductNameCommand(
    Guid ProductId,
    string ProductName) : IRequest<UpdateProductNameResultDto>;