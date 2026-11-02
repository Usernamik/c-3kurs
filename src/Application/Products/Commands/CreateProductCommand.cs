using Application.Common.Interfaces.Repositories;
using Domain.Products;
using MediatR;

namespace Application.Products.Commands;

public record CreateProductCommand : IRequest<Product>
{
    public required string Title { get; init; }
    public required decimal Price { get; init; }
}

public class CreateProductCommandHandler(IProductRepository productRepository)
    : IRequestHandler<CreateProductCommand, Product>
{
    public async Task<Product> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var existingProduct = await productRepository.GetByTitle(request.Title, cancellationToken);
        if (existingProduct is not null)
        {
            throw new ArgumentException($"Product with title '{request.Title}' already exists");
        }

        var product = Product.New(Guid.NewGuid(), request.Title, request.Price);
        return await productRepository.Add(product, cancellationToken);
    }
}
