using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Products;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ProductRepository(ApplicationDbContext context) : IProductRepository, IProductQueries
{
    public async Task<IReadOnlyList<Product>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Products
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<Product>> GetById(ProductId id, CancellationToken cancellationToken)
    {
        var entity = await context.Products
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity ?? Option<Product>.None;
    }

    public async Task<Option<Product>> GetByTitle(string title, CancellationToken cancellationToken)
    {
        var entity = await context.Products
            .FirstOrDefaultAsync(x => x.Title == title, cancellationToken);

        return entity ?? Option<Product>.None;
    }

    public async Task<Product> Add(Product product, CancellationToken cancellationToken)
    {
        await context.Products.AddAsync(product, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<Product> Update(Product product, CancellationToken cancellationToken)
    {
        context.Products.Update(product);
        await context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task Delete(Product product, CancellationToken cancellationToken)
    {
        context.Products.Remove(product);
        await context.SaveChangesAsync(cancellationToken);
    }
}
