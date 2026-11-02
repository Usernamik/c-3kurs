using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Apartments;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ApartmentRepository(ApplicationDbContext context) : IApartmentRepository, IApartmentQueries
{
    public async Task<IReadOnlyList<Apartment>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Apartments
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<Apartment>> GetById(ApartmentId id, CancellationToken cancellationToken)
    {
        var entity = await context.Apartments
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity ?? Option<Apartment>.None;
    }

    public async Task<Option<Apartment>> GetByAddress(string address, CancellationToken cancellationToken)
    {
        var entity = await context.Apartments
            .FirstOrDefaultAsync(x => x.Address == address, cancellationToken);

        return entity ?? Option<Apartment>.None;
    }

    public async Task<Apartment> Add(Apartment apartment, CancellationToken cancellationToken)
    {
        await context.Apartments.AddAsync(apartment, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return apartment;
    }

    public async Task<Apartment> Update(Apartment apartment, CancellationToken cancellationToken)
    {
        context.Apartments.Update(apartment);
        await context.SaveChangesAsync(cancellationToken);
        return apartment;
    }

    public async Task Delete(Apartment apartment, CancellationToken cancellationToken)
    {
        context.Apartments.Remove(apartment);
        await context.SaveChangesAsync(cancellationToken);
    }
}
