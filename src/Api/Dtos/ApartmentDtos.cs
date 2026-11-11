using Domain.Apartments;

namespace Api.Dtos;

public record ApartmentDto(Guid Id, string Address, decimal Area, int RoomsCount, decimal Price, bool IsSold, DateTime CreatedAt)
{
    public static ApartmentDto FromDomainModel(Apartment apartment)
        => new(apartment.Id.Value, apartment.Address, apartment.Area, apartment.RoomsCount, apartment.Price, apartment.IsSold, apartment.CreatedAt);
}

public record CreateApartmentDto(
    string Address,
    decimal Area,
    int RoomsCount,
    decimal Price,
    bool IsSold = false);

public record UpdateApartmentDto(
    string Address,
    decimal Area,
    int RoomsCount,
    decimal Price,
    bool IsSold = false);

public record SearchApartmentDto(
    string? Address = null,
    decimal? Area = null,
    int? RoomsCount = null,
    decimal? Price = null,
    bool? IsSold = null,
    DateTime? CreatedAt = null);
