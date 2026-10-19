using System.Net;
using System.Text;
using System.Text.Json;

namespace CSharpLearningApi.Tests;

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

        var addResponse = await client.PostAsync("/products", new StringContent(
            @"{
                ""name"": ""New Product"",
                ""price"": 9.99
            }",
            Encoding.UTF8,
            "application/json"
        ));

        Assert.Equal(HttpStatusCode.Created, addResponse.StatusCode);
    }

    [Fact]
    public async Task Update_existing_product_updates_it()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var updateResponse = await client.PutAsync("/products/1", new StringContent(
            @"{
                ""name"": ""Updated Product"",
                ""price"": 9.99
            }",
            Encoding.UTF8,
            "application/json"
        ));

        using var jsonDocument = JsonDocument.Parse(
            await updateResponse.Content.ReadAsStringAsync());

        var product = jsonDocument.RootElement;

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.Equal(1, product.GetProperty("id").GetInt32());
        Assert.Equal("Updated Product", product.GetProperty("name").GetString());
        Assert.Equal(9.99m, product.GetProperty("price").GetDecimal());
    }

    [Fact]
    public async Task Update_non_existing_product_returns_not_found()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var updateResponse = await client.PutAsync("/products/999", new StringContent(
            @"{
                ""name"": ""Updated Product"",
                ""price"": 9.99
            }",
            Encoding.UTF8,
            "application/json"
        ));

        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);
    }

    [Fact]
    public async Task Get_existing_product_returns_ok()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var getResponse = await client.GetAsync("/products/1");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task Get_non_existing_product_returns_not_found()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var getResponse = await client.GetAsync("/products/999");

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}