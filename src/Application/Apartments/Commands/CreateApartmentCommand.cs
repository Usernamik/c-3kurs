using Application.Common.Interfaces.Repositories;
using Domain.Apartments;
using MediatR;

namespace Application.Apartments.Commands;

public record CreateApartmentCommand : IRequest<Apartment>
{
    public required string Address { get; init; }
    public required decimal Area { get; init; }
    public required int RoomsCount { get; init; }
    public required decimal Price { get; init; }
    public required bool IsSold { get; init; }
}

public class CreateApartmentCommandHandler(IApartmentRepository apartmentRepository)
    : IRequestHandler<CreateApartmentCommand, Apartment>
{
    public async Task<Apartment> Handle(
        CreateApartmentCommand request,
        CancellationToken cancellationToken)
    {
        var existingApartment = await apartmentRepository.GetByAddress(request.Address, cancellationToken);
        if (existingApartment is not null)
        {
            throw new ArgumentException($"Apartment with address '{request.Address}' already exists");
        }

        var apartment = Apartment.New(
            Guid.NewGuid(),
            request.Address,
            request.Area,
            request.RoomsCount,
            request.Price,
            request.IsSold);

        return await apartmentRepository.Add(apartment, cancellationToken);
    }
}
