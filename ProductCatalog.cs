using Microsoft.EntityFrameworkCore;

internal interface IProductCatalog
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken);
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Product> AddAsync(string name, decimal price, CancellationToken cancellationToken);
    Task<Product?> UpdateAsync(int id, string name, decimal price, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}

internal sealed class ProductCatalog : IProductCatalog
{
    private readonly AppDbContext _dbContext;

    public ProductCatalog(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .OrderBy(product => product.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return _dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public async Task<Product> AddAsync(
        string name,
        decimal price,
        CancellationToken cancellationToken)
    {
        var newProduct = new Product(name, price);
        _dbContext.Products.Add(newProduct);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return newProduct;
    }

    public async Task<Product?> UpdateAsync(
        int id,
        string name,
        decimal price,
        CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);

        if (product is null)
        {
            return null;
        }

        product.UpdateDetails(name, price);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);

        if (product is null)
        {
            return false;
        }

        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}