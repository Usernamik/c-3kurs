namespace Domain.Apartments;

public class Apartment
{
    public ApartmentId Id { get; }
    public string Address { get; private set; }
    public decimal Area { get; private set; }
    public int RoomsCount { get; private set; }
    public decimal Price { get; private set; }
    public bool IsSold { get; private set; }
    public DateTime CreatedAt { get; }

    private Apartment(ApartmentId id, string address, decimal area, int roomsCount, decimal price, bool isSold, DateTime createdAt)
        => (Id, Address, Area, RoomsCount, Price, IsSold, CreatedAt) = (id, address, area, roomsCount, price, isSold, createdAt);

    public static Apartment New(ApartmentId id, string address, decimal area, int roomsCount, decimal price, bool isSold = false)
        => new(id, address, area, roomsCount, price, isSold, DateTime.UtcNow);

    public void UpdateDetails(string address, decimal area, int roomsCount, decimal price, bool isSold)
        => (Address, Area, RoomsCount, Price, IsSold) = (address, area, roomsCount, price, isSold);

    public void MarkAsSold() => IsSold = true;
}
