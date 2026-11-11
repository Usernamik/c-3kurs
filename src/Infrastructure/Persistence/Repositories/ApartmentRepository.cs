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

    public async Task<IReadOnlyList<Apartment>> Filter(
        string? address,
        decimal? area,
        int? roomsCount,
        decimal? price,
        bool? isSold,
        DateTime? createdAt,
        CancellationToken cancellationToken)
    {
        var query = context.Apartments.AsNoTracking();

        if (!string.IsNullOrEmpty(address))
        {
            var addressFilter = address.Trim().ToLower();
            query = query.Where(x => x.Address.ToLower().Contains(addressFilter));
        }

        if (area is not null)
        {
            query = query.Where(x => x.Area == area);
        }

        if (roomsCount is not null)
        {
            query = query.Where(x => x.RoomsCount == roomsCount);
        }

        if (price is not null)
        {
            query = query.Where(x => x.Price == price);
        }

        if (isSold is not null)
        {
            query = query.Where(x => x.IsSold == isSold);
        }

        if (createdAt is not null)
        {
            var start = DateTime.SpecifyKind(createdAt.Value.Date, DateTimeKind.Utc);
            var end = start.AddDays(1);
            query = query.Where(x => x.CreatedAt >= start && x.CreatedAt < end);
        }

        return await query.ToListAsync(cancellationToken);
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
