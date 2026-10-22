using System.ComponentModel.DataAnnotations;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        app.MapGet("/products", async (
            IProductCatalog catalog,
            CancellationToken cancellationToken,
            [Range(1, 100)] int page = 1,
            [Range(1, 100)] int pageSize = 10) =>
        {
            var products = await catalog.GetPageAsync(page, pageSize, cancellationToken);
            var totalCount = await catalog.GetCountAsync(cancellationToken);
            return Results.Ok(new PagedProductsResponse(
                products.Select(p => new ProductResponse(p.Id, p.Name, p.Price)).ToList(),
                page,
                pageSize,
                totalCount));
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

        app.MapPost("/products", async (
            CreateProductRequest request,
            IProductCatalog catalog,
            CancellationToken cancellationToken) =>
        {
            var newProduct = await catalog.AddAsync(request.Name, request.Price, cancellationToken);
            return TypedResults.Created(
            $"/products/{newProduct.Id}",
            new ProductResponse(newProduct.Id, newProduct.Name, newProduct.Price));
        })
            .WithName("CreateProduct");

        app.MapPut("/products/{id:int}", async Task<IResult> (
            int id,
            UpdateProductRequest request,
            IProductCatalog catalog,
            CancellationToken cancellationToken) =>
        {
            var updatedProduct = await catalog.UpdateAsync(id, request.Name, request.Price, cancellationToken);
            if (updatedProduct is null)
            {
                return Results.NotFound();
            }
            return TypedResults.Ok(new ProductResponse(updatedProduct.Id, updatedProduct.Name, updatedProduct.Price));
        })
            .WithName("UpdateProduct");

        app.MapDelete("/products/{id:int}", async Task<IResult> (
            int id,
            IProductCatalog catalog,
            CancellationToken cancellationToken) =>
        {
            var deleted = await catalog.DeleteAsync(id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
            .WithName("DeleteProduct");
    }
}