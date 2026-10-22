using System.Net;
using System.Net.Http.Json;

namespace CSharpLearningApi.Tests;

public sealed record ProductResponse(int Id, string Name, decimal Price);

public class ProductEndpointTests
{
    [Fact]
    public async Task Delete_existing_product_removes_it()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var deleteResponse = await client.DeleteAsync("/products/1");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync("/products/1");

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_non_existing_product_returns_not_found()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var deleteResponse = await client.DeleteAsync("/products/999");

        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync("/products/999");

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Add_product_adds_it()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var addResponse = await client.PostAsJsonAsync("/products", new
        {
            name = "New Product",
            price = 9.99m
        });

        Assert.Equal(HttpStatusCode.Created, addResponse.StatusCode);
        Assert.NotNull(addResponse.Headers.Location);

        var getResponse = await client.GetAsync(addResponse.Headers.Location);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var product = await getResponse.Content.ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(product);
        Assert.Equal("New Product", product.Name);
        Assert.Equal(9.99m, product.Price);
    }

    [Fact]
    public async Task Update_existing_product_updates_it()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var updateResponse = await client.PutAsJsonAsync("/products/1", new
        {
            name = "Updated Product",
            price = 9.99m
        });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var product = await updateResponse.Content.ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(product);
        Assert.Equal(1, product.Id);
        Assert.Equal("Updated Product", product.Name);
        Assert.Equal(9.99m, product.Price);

        var getResponse = await client.GetAsync("/products/1");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var getProduct = await getResponse.Content.ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(getProduct);
        Assert.Equal(1, getProduct.Id);
        Assert.Equal("Updated Product", getProduct.Name);
        Assert.Equal(9.99m, getProduct.Price);
    }

    [Fact]
    public async Task Update_non_existing_product_returns_not_found()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var updateResponse = await client.PutAsJsonAsync("/products/999", new
        {
            name = "Updated Product",
            price = 9.99m
        });

        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);
    }

    [Fact]
    public async Task Get_existing_product_returns_ok()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var getResponse = await client.GetAsync("/products/1");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var product = await getResponse.Content.ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(product);
        Assert.Equal(1, product.Id);
        Assert.Equal("Keyboard", product.Name);
        Assert.Equal(79.99m, product.Price);
    }

    [Fact]
    public async Task Get_non_existing_product_returns_not_found()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var getResponse = await client.GetAsync("/products/999");

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Theory]
    [InlineData("", 9.99)]
    [InlineData("Bad Price", 0)]
    public async Task Add_invalid_product_returns_bad_request(string name, decimal price)
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/products", new
        {
            name,
            price
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(999999.99)]
    public async Task Add_product_at_price_boundary_returns_created(decimal price)
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/products", new
        {
            name = "Boundary Product",
            price
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}