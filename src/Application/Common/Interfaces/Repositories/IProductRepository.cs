using Domain.Products;
using LanguageExt;

namespace Application.Common.Interfaces.Repositories;

public interface IProductRepository
{
    Task<Option<Product>> GetById(ProductId id, CancellationToken cancellationToken);
    Task<Option<Product>> GetByTitle(string title, CancellationToken cancellationToken);
    Task<Product> Add(Product product, CancellationToken cancellationToken);
    Task<Product> Update(Product product, CancellationToken cancellationToken);
    Task Delete(Product product, CancellationToken cancellationToken);
}
