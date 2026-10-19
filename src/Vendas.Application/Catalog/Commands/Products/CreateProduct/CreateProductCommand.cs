using MediatR;

namespace Vendas.Application.Catalog.Commands.Products.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string Code,
    decimal Price,
    Guid CategoryId,
    int InitialStock = 0,
    string? Description = null) : IRequest<CreateProductResultDto>;