using Api.Dtos;
using Application.Common.Interfaces.Queries;
using Application.Products.Commands;
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
        var product = await productQueries.GetById(id, cancellationToken);
        if (product is null)
        {
            return NotFound();
        }

        return ProductDto.FromDomainModel(product);
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

        try
        {
            var product = await sender.Send(input, cancellationToken);
            var dto = ProductDto.FromDomainModel(product);
            return CreatedAtAction(nameof(GetProduct), new { id = dto.Id }, dto);
        }
        catch (ArgumentException exception)
        {
            return Conflict(exception.Message);
        }
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

        try
        {
            var product = await sender.Send(input, cancellationToken);
            if (product is null)
            {
                return NotFound();
            }

            return ProductDto.FromDomainModel(product);
        }
        catch (ArgumentException exception)
        {
            return Conflict(exception.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
    {
        var input = new DeleteProductCommand
        {
            ProductId = id
        };

        var deleted = await sender.Send(input, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
