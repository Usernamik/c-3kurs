using Application.Common.Interfaces.Repositories;
using Application.Products.Exceptions;
using Domain.Products;
using LanguageExt;
using MediatR;

namespace Application.Products.Commands;

public record CreateProductCommand : IRequest<Either<ProductException, Product>>
{
    public required string Title { get; init; }
    public required decimal Price { get; init; }
}

public class CreateProductCommandHandler(IProductRepository productRepository)
    : IRequestHandler<CreateProductCommand, Either<ProductException, Product>>
{
    public async Task<Either<ProductException, Product>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var existingProduct = await productRepository.GetByTitle(request.Title, cancellationToken);

        return await existingProduct.MatchAsync(
            product => new ProductAlreadyExistsException(product.Id),
            () => CreateEntity(request, cancellationToken));
    }

    private async Task<Either<ProductException, Product>> CreateEntity(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var product = Product.New(ProductId.New(), request.Title, request.Price);
            return await productRepository.Add(product, cancellationToken);
        }
        catch (Exception exception)
        {
            return new ProductUnhandledException(ProductId.Empty(), exception);
        }
    }
}
