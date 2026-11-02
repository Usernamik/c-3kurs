using Domain.Apartments;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IApartmentQueries
{
    Task<IReadOnlyList<Apartment>> GetAll(CancellationToken cancellationToken);
    Task<Option<Apartment>> GetById(ApartmentId id, CancellationToken cancellationToken);
}
