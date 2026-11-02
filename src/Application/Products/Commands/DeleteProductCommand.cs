using Application.Common.Interfaces.Repositories;
using MediatR;

namespace Application.Products.Commands;

public record DeleteProductCommand : IRequest<bool>
{
    public required Guid ProductId { get; init; }
}

public class DeleteProductCommandHandler(IProductRepository productRepository)
    : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetById(request.ProductId, cancellationToken);
        if (product is null)
        {
            return false;
        }

        await productRepository.Delete(product, cancellationToken);
        return true;
    }
}
