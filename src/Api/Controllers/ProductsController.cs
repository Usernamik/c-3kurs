using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Products.Commands;
using Domain.Products;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("products")]
[ApiController]
public class ProductsController(ISender sender, IProductQueries productQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetProducts(CancellationToken cancellationToken)
    {
        var products = await productQueries.GetAll(cancellationToken);
        return products.Select(ProductDto.FromDomainModel).ToList();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id, CancellationToken cancellationToken)
    {
        var product = await productQueries.GetById(new ProductId(id), cancellationToken);

        return product.Match<ActionResult<ProductDto>>(
            current => ProductDto.FromDomainModel(current),
            () => NotFound());
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(
        [FromBody] CreateProductDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateProductCommand
        {
            Title = request.Title,
            Price = request.Price
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<ProductDto>>(
            product => CreatedAtAction(
                nameof(GetProduct),
                new { id = product.Id.Value },
                ProductDto.FromDomainModel(product)),
            error => error.ToObjectResult());
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(
        Guid id,
        [FromBody] UpdateProductDto request,
        CancellationToken cancellationToken)
    {
        var input = new UpdateProductCommand
        {
            ProductId = id,
            Title = request.Title,
            Price = request.Price
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<ProductDto>>(
            product => ProductDto.FromDomainModel(product),
            error => error.ToObjectResult());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
    {
        var input = new DeleteProductCommand
        {
            ProductId = id
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<IActionResult>(
            _ => NoContent(),
            error => error.ToObjectResult());
    }
}
