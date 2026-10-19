internal sealed class Product
{
    private Product()
    {
    }

    public Product(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    public void UpdateDetails(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public decimal PriceWithTax => Price * 1.1m;
}
