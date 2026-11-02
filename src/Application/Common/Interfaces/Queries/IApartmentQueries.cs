using Domain.Apartments;

namespace Application.Common.Interfaces.Queries;

public interface IApartmentQueries
{
    Task<IReadOnlyList<Apartment>> GetAll(CancellationToken cancellationToken);
    Task<Apartment?> GetById(Guid id, CancellationToken cancellationToken);
}
