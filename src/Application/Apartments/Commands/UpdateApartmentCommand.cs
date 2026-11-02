using Application.Common.Interfaces.Repositories;
using Domain.Apartments;
using MediatR;

namespace Application.Apartments.Commands;

public record UpdateApartmentCommand : IRequest<Apartment?>
{
    public required Guid ApartmentId { get; init; }
    public required string Address { get; init; }
    public required decimal Area { get; init; }
    public required int RoomsCount { get; init; }
    public required decimal Price { get; init; }
    public required bool IsSold { get; init; }
}

public class UpdateApartmentCommandHandler(IApartmentRepository apartmentRepository)
    : IRequestHandler<UpdateApartmentCommand, Apartment?>
{
    public async Task<Apartment?> Handle(
        UpdateApartmentCommand request,
        CancellationToken cancellationToken)
    {
        var apartment = await apartmentRepository.GetById(request.ApartmentId, cancellationToken);
        if (apartment is null)
        {
            return null;
        }

        var apartmentWithSameAddress = await apartmentRepository.GetByAddress(request.Address, cancellationToken);
        if (apartmentWithSameAddress is not null && apartmentWithSameAddress.Id != request.ApartmentId)
        {
            throw new ArgumentException($"Apartment with address '{request.Address}' already exists");
        }

        apartment.UpdateDetails(request.Address, request.Area, request.RoomsCount, request.Price, request.IsSold);
        return await apartmentRepository.Update(apartment, cancellationToken);
    }
}
