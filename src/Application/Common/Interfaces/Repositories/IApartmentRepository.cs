using Domain.Apartments;

namespace Application.Common.Interfaces.Repositories;

public interface IApartmentRepository
{
    Task<Apartment?> GetById(Guid id, CancellationToken cancellationToken);
    Task<Apartment?> GetByAddress(string address, CancellationToken cancellationToken);
    Task<Apartment> Add(Apartment apartment, CancellationToken cancellationToken);
    Task<Apartment> Update(Apartment apartment, CancellationToken cancellationToken);
    Task Delete(Apartment apartment, CancellationToken cancellationToken);
}
