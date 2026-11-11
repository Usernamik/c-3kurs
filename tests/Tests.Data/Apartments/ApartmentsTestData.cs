using Domain.Apartments;

namespace Tests.Data.Apartments;

public static class ApartmentsTestData
{
    public static Apartment FirstTestApartment() => Apartment.New(
        ApartmentId.New(),
        "First test apartment",
        55.5m,
        2,
        100_000m);

    public static Apartment SecondTestApartment() => Apartment.New(
        ApartmentId.New(),
        "Second test apartment",
        80m,
        3,
        200_000m);

    public static Apartment ThirdTestApartment() => Apartment.New(
        ApartmentId.New(),
        "Third test apartment",
        120m,
        4,
        300_000m);
}
