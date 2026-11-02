using Domain.Apartments;
using LanguageExt;

namespace Application.Common.Interfaces.Repositories;

public interface IApartmentRepository
{
    Task<Option<Apartment>> GetById(ApartmentId id, CancellationToken cancellationToken);
    Task<Option<Apartment>> GetByAddress(string address, CancellationToken cancellationToken);
    Task<Apartment> Add(Apartment apartment, CancellationToken cancellationToken);
    Task<Apartment> Update(Apartment apartment, CancellationToken cancellationToken);
    Task Delete(Apartment apartment, CancellationToken cancellationToken);
}
