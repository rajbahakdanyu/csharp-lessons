using Microsoft.EntityFrameworkCore;

internal sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new { Id = 1, Name = "Keyboard", Price = 79.99m },
            new { Id = 2, Name = "Mouse", Price = 24.50m },
            new { Id = 3, Name = "Monitor", Price = 249.99m });
    }
}