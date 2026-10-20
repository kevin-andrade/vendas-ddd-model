using MediatR;

namespace Vendas.Application.Catalog.Commands.Products.ActivateProduct;

public sealed record ActivateProductCommand(Guid ProductId) :  IRequest<ActivateProductResultDto>;