using Application.Common.Interfaces.Repositories;
using Application.Products.Exceptions;
using Domain.Products;
using LanguageExt;
using MediatR;
using Unit = LanguageExt.Unit;

namespace Application.Products.Commands;

public record UpdateProductCommand : IRequest<Either<ProductException, Product>>
{
    public required Guid ProductId { get; init; }
    public required string Title { get; init; }
    public required decimal Price { get; init; }
}

public class UpdateProductCommandHandler(IProductRepository productRepository)
    : IRequestHandler<UpdateProductCommand, Either<ProductException, Product>>
{
    public async Task<Either<ProductException, Product>> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var productId = new ProductId(request.ProductId);
        var product = await productRepository.GetById(productId, cancellationToken);

        return await product.MatchAsync(
            current => CheckDuplicates(current.Id, request.Title, cancellationToken)
                .BindAsync(_ => UpdateEntity(current, request, cancellationToken)),
            () => new ProductNotFoundException(productId));
    }

    private async Task<Either<ProductException, Unit>> CheckDuplicates(
        ProductId currentProductId,
        string title,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByTitle(title, cancellationToken);

        return product.Match<Either<ProductException, Unit>>(
            existing => existing.Id == currentProductId ? Unit.Default : new ProductAlreadyExistsException(existing.Id),
            () => Unit.Default);
    }

    private async Task<Either<ProductException, Product>> UpdateEntity(
        Product product,
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            product.UpdateDetails(request.Title, request.Price);
            return await productRepository.Update(product, cancellationToken);
        }
        catch (Exception exception)
        {
            return new ProductUnhandledException(product.Id, exception);
        }
    }
}
