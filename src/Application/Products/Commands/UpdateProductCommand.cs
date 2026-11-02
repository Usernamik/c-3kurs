using Application.Common.Interfaces.Repositories;
using Domain.Products;
using MediatR;

namespace Application.Products.Commands;

public record UpdateProductCommand : IRequest<Product?>
{
    public required Guid ProductId { get; init; }
    public required string Title { get; init; }
    public required decimal Price { get; init; }
}

public class UpdateProductCommandHandler(IProductRepository productRepository)
    : IRequestHandler<UpdateProductCommand, Product?>
{
    public async Task<Product?> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetById(request.ProductId, cancellationToken);
        if (product is null)
        {
            return null;
        }

        var productWithSameTitle = await productRepository.GetByTitle(request.Title, cancellationToken);
        if (productWithSameTitle is not null && productWithSameTitle.Id != request.ProductId)
        {
            throw new ArgumentException($"Product with title '{request.Title}' already exists");
        }

        product.UpdateDetails(request.Title, request.Price);
        return await productRepository.Update(product, cancellationToken);
    }
}
