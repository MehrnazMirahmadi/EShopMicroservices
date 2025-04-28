namespace Ordering.Domain.Models;

public class Product
{
    public string Name { get; private set; } = default!;
    public decimal Price { get; private set; } = default!;
}
