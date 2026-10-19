using System.Net;

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
            System.Text.Encoding.UTF8,
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
            System.Text.Encoding.UTF8,
            "application/json"
        ));

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
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
            System.Text.Encoding.UTF8,
            "application/json"
        ));

        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);
    }
}