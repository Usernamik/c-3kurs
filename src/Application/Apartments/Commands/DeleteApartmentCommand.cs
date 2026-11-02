using Application.Common.Interfaces.Repositories;
using MediatR;

namespace Application.Apartments.Commands;

public record DeleteApartmentCommand : IRequest<bool>
{
    public required Guid ApartmentId { get; init; }
}

public class DeleteApartmentCommandHandler(IApartmentRepository apartmentRepository)
    : IRequestHandler<DeleteApartmentCommand, bool>
{
    public async Task<bool> Handle(
        DeleteApartmentCommand request,
        CancellationToken cancellationToken)
    {
        var apartment = await apartmentRepository.GetById(request.ApartmentId, cancellationToken);
        if (apartment is null)
        {
            return false;
        }

        await apartmentRepository.Delete(apartment, cancellationToken);
        return true;
    }
}
