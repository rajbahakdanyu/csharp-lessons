public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        app.MapGet("/products", async (IProductCatalog catalog, CancellationToken cancellationToken) =>
        {
            var products = await catalog.GetAllAsync(cancellationToken);
            return Results.Ok(products.Select(p => new ProductResponse(p.Id, p.Name, p.Price)));
        })
        .WithName("GetProducts");

        app.MapGet("/products/{id:int}", async Task<IResult> (
            int id,
            IProductCatalog catalog,
            CancellationToken cancellationToken) =>
        {
            var product = await catalog.GetByIdAsync(id, cancellationToken);
            return product is null
            ? Results.NotFound()
            : Results.Ok(new ProductResponse(product.Id, product.Name, product.Price));
        })
            .WithName("GetProductById");
    }
}