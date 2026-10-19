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
}