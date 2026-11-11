using Api.Dtos;
using Api.Modules.Errors;
using Application.Apartments.Commands;
using Application.Common.Interfaces.Queries;
using Domain.Apartments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("apartments")]
[ApiController]
public class ApartmentsController(
    ISender sender,
    IApartmentQueries apartmentQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ApartmentDto>>> GetApartments(CancellationToken cancellationToken)
    {
        var apartments = await apartmentQueries.GetAll(cancellationToken);
        return apartments.Select(ApartmentDto.FromDomainModel).ToList();
    }

    [HttpPost("filter")]
    public async Task<ActionResult<IReadOnlyList<ApartmentDto>>> FilterApartments(
        [FromBody] SearchApartmentDto request,
        CancellationToken cancellationToken)
    {
        var apartments = await apartmentQueries.Filter(
            request.Address,
            request.Area,
            request.RoomsCount,
            request.Price,
            request.IsSold,
            request.CreatedAt,
            cancellationToken);

        return apartments.Select(ApartmentDto.FromDomainModel).ToList();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApartmentDto>> GetApartment(Guid id, CancellationToken cancellationToken)
    {
        var apartment = await apartmentQueries.GetById(new ApartmentId(id), cancellationToken);

        return apartment.Match<ActionResult<ApartmentDto>>(
            current => ApartmentDto.FromDomainModel(current),
            () => NotFound());
    }

    [HttpPost]
    public async Task<ActionResult<ApartmentDto>> CreateApartment(
        [FromBody] CreateApartmentDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateApartmentCommand
        {
            Address = request.Address,
            Area = request.Area,
            RoomsCount = request.RoomsCount,
            Price = request.Price,
            IsSold = request.IsSold
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<ApartmentDto>>(
            apartment => CreatedAtAction(
                nameof(GetApartment),
                new { id = apartment.Id.Value },
                ApartmentDto.FromDomainModel(apartment)),
            error => error.ToObjectResult());
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApartmentDto>> UpdateApartment(
        Guid id,
        [FromBody] UpdateApartmentDto request,
        CancellationToken cancellationToken)
    {
        var input = new UpdateApartmentCommand
        {
            ApartmentId = id,
            Address = request.Address,
            Area = request.Area,
            RoomsCount = request.RoomsCount,
            Price = request.Price,
            IsSold = request.IsSold
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<ApartmentDto>>(
            apartment => ApartmentDto.FromDomainModel(apartment),
            error => error.ToObjectResult());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteApartment(Guid id, CancellationToken cancellationToken)
    {
        var input = new DeleteApartmentCommand
        {
            ApartmentId = id
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<IActionResult>(
            _ => NoContent(),
            error => error.ToObjectResult());
    }
}
