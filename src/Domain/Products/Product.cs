namespace Domain.Products;

public class Product
{
    public Guid Id { get; }
    public string Title { get; private set; }
    public decimal Price { get; private set; }
    public DateTime CreatedAt { get; }

    private Product(Guid id, string title, decimal price, DateTime createdAt)
        => (Id, Title, Price, CreatedAt) = (id, title, price, createdAt);

    public static Product New(Guid id, string title, decimal price)
        => new(id, title, price, DateTime.UtcNow);

    public void UpdateDetails(string title, decimal price)
        => (Title, Price) = (title, price);
}
