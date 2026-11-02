using Application.Apartments.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Apartments;
using LanguageExt;
using MediatR;

namespace Application.Apartments.Commands;

public record DeleteApartmentCommand : IRequest<Either<ApartmentException, Apartment>>
{
    public required Guid ApartmentId { get; init; }
}

public class DeleteApartmentCommandHandler(IApartmentRepository apartmentRepository)
    : IRequestHandler<DeleteApartmentCommand, Either<ApartmentException, Apartment>>
{
    public async Task<Either<ApartmentException, Apartment>> Handle(
        DeleteApartmentCommand request,
        CancellationToken cancellationToken)
    {
        var apartmentId = new ApartmentId(request.ApartmentId);
        var apartment = await apartmentRepository.GetById(apartmentId, cancellationToken);

        return await apartment.MatchAsync(
            current => DeleteEntity(current, cancellationToken),
            () => new ApartmentNotFoundException(apartmentId));
    }

    private async Task<Either<ApartmentException, Apartment>> DeleteEntity(
        Apartment apartment,
        CancellationToken cancellationToken)
    {
        try
        {
            await apartmentRepository.Delete(apartment, cancellationToken);
            return apartment;
        }
        catch (Exception exception)
        {
            return new ApartmentUnhandledException(apartment.Id, exception);
        }
    }
}
