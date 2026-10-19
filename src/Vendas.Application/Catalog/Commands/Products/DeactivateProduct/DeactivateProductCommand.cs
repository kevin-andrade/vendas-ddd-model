using MediatR;

namespace Vendas.Application.Catalog.Commands.Products.DeactivateProduct;

public sealed record DeactivateProductCommand(
    Guid ProductId,
    string Status) : IRequest<DeactivateProductResultDto>;