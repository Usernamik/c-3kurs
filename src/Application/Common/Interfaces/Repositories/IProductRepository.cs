using Domain.Products;

namespace Application.Common.Interfaces.Repositories;

public interface IProductRepository
{
    Task<Product?> GetById(Guid id, CancellationToken cancellationToken);
    Task<Product?> GetByTitle(string title, CancellationToken cancellationToken);
    Task<Product> Add(Product product, CancellationToken cancellationToken);
    Task<Product> Update(Product product, CancellationToken cancellationToken);
    Task Delete(Product product, CancellationToken cancellationToken);
}
