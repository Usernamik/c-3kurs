using Application.Apartments.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Apartments;
using LanguageExt;
using MediatR;

namespace Application.Apartments.Commands;

public record CreateApartmentCommand : IRequest<Either<ApartmentException, Apartment>>
{
    public required string Address { get; init; }
    public required decimal Area { get; init; }
    public required int RoomsCount { get; init; }
    public required decimal Price { get; init; }
    public required bool IsSold { get; init; }
}

public class CreateApartmentCommandHandler(IApartmentRepository apartmentRepository)
    : IRequestHandler<CreateApartmentCommand, Either<ApartmentException, Apartment>>
{
    public async Task<Either<ApartmentException, Apartment>> Handle(
        CreateApartmentCommand request,
        CancellationToken cancellationToken)
    {
        var existingApartment = await apartmentRepository.GetByAddress(request.Address, cancellationToken);

        return await existingApartment.MatchAsync(
            apartment => new ApartmentAlreadyExistsException(apartment.Id),
            () => CreateEntity(request, cancellationToken));
    }

    private async Task<Either<ApartmentException, Apartment>> CreateEntity(
        CreateApartmentCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var apartment = Apartment.New(
                ApartmentId.New(),
                request.Address,
                request.Area,
                request.RoomsCount,
                request.Price,
                request.IsSold);

            return await apartmentRepository.Add(apartment, cancellationToken);
        }
        catch (Exception exception)
        {
            return new ApartmentUnhandledException(ApartmentId.Empty(), exception);
        }
    }
}
