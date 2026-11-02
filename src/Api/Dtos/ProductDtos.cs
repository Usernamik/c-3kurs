using Domain.Products;

namespace Api.Dtos;

public record ProductDto(Guid Id, string Title, decimal Price, DateTime CreatedAt)
{
    public static ProductDto FromDomainModel(Product product)
        => new(product.Id.Value, product.Title, product.Price, product.CreatedAt);
}

public record CreateProductDto(string Title, decimal Price);

public record UpdateProductDto(string Title, decimal Price);
