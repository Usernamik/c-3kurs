namespace Domain.Apartments;

public record ApartmentId(Guid Value)
{
    public static ApartmentId Empty() => new(Guid.Empty);
    public static ApartmentId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}