using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddValidation();
builder.Services.AddScoped<IProductCatalog, ProductCatalog>();
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlite(
        builder.Configuration.GetConnectionString("Products") ?? "Data Source=products.db"));

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/products", async (IProductCatalog catalog, CancellationToken cancellationToken) =>
    await catalog.GetAllAsync(cancellationToken))
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

app.Run();

public sealed record CreateProductRequest(
    [Required][StringLength(100)] string Name,
    [Range(0.01, 999999.99)] decimal Price);

public sealed record UpdateProductRequest(
    [Required][StringLength(100)] string Name,
    [Range(0.01, 999999.99)] decimal Price);

public sealed record ProductResponse(int Id, string Name, decimal Price);