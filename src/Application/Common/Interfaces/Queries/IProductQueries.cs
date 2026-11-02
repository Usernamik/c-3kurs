using Domain.Products;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IProductQueries
{
    Task<IReadOnlyList<Product>> GetAll(CancellationToken cancellationToken);
    Task<Option<Product>> GetById(ProductId id, CancellationToken cancellationToken);
}
