using Application.Apartments.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Apartments;
using LanguageExt;
using MediatR;
using Unit = LanguageExt.Unit;

namespace Application.Apartments.Commands;

public record UpdateApartmentCommand : IRequest<Either<ApartmentException, Apartment>>
{
    public required Guid ApartmentId { get; init; }
    public required string Address { get; init; }
    public required decimal Area { get; init; }
    public required int RoomsCount { get; init; }
    public required decimal Price { get; init; }
    public required bool IsSold { get; init; }
}

public class UpdateApartmentCommandHandler(IApartmentRepository apartmentRepository)
    : IRequestHandler<UpdateApartmentCommand, Either<ApartmentException, Apartment>>
{
    public async Task<Either<ApartmentException, Apartment>> Handle(
        UpdateApartmentCommand request,
        CancellationToken cancellationToken)
    {
        var apartmentId = new ApartmentId(request.ApartmentId);
        var apartment = await apartmentRepository.GetById(apartmentId, cancellationToken);

        return await apartment.MatchAsync(
            current => CheckDuplicates(current.Id, request.Address, cancellationToken)
                .BindAsync(_ => UpdateEntity(current, request, cancellationToken)),
            () => new ApartmentNotFoundException(apartmentId));
    }

    private async Task<Either<ApartmentException, Unit>> CheckDuplicates(
        ApartmentId currentApartmentId,
        string address,
        CancellationToken cancellationToken)
    {
        var apartment = await apartmentRepository.GetByAddress(address, cancellationToken);

        return apartment.Match<Either<ApartmentException, Unit>>(
            existing => existing.Id == currentApartmentId
                ? Unit.Default
                : new ApartmentAlreadyExistsException(existing.Id),
            () => Unit.Default);
    }

    private async Task<Either<ApartmentException, Apartment>> UpdateEntity(
        Apartment apartment,
        UpdateApartmentCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            apartment.UpdateDetails(request.Address, request.Area, request.RoomsCount, request.Price, request.IsSold);
            return await apartmentRepository.Update(apartment, cancellationToken);
        }
        catch (Exception exception)
        {
            return new ApartmentUnhandledException(apartment.Id, exception);
        }
    }
}
