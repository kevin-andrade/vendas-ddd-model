using MediatR;

namespace Vendas.Application.Catalog.Commands.Categories.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    string Description) : IRequest<CreateCategoryResultDto>;