using Api.Dtos;
using Application.Apartments.Commands;
using Application.Common.Interfaces.Queries;
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

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApartmentDto>> GetApartment(Guid id, CancellationToken cancellationToken)
    {
        var apartment = await apartmentQueries.GetById(id, cancellationToken);
        if (apartment is null)
        {
            return NotFound();
        }

        return ApartmentDto.FromDomainModel(apartment);
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

        try
        {
            var apartment = await sender.Send(input, cancellationToken);
            var dto = ApartmentDto.FromDomainModel(apartment);
            return CreatedAtAction(nameof(GetApartment), new { id = dto.Id }, dto);
        }
        catch (ArgumentException exception)
        {
            return Conflict(exception.Message);
        }
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

        try
        {
            var apartment = await sender.Send(input, cancellationToken);
            if (apartment is null)
            {
                return NotFound();
            }

            return ApartmentDto.FromDomainModel(apartment);
        }
        catch (ArgumentException exception)
        {
            return Conflict(exception.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteApartment(Guid id, CancellationToken cancellationToken)
    {
        var input = new DeleteApartmentCommand
        {
            ApartmentId = id
        };

        var deleted = await sender.Send(input, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
