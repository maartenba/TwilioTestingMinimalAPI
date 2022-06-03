using Microsoft.AspNetCore.Mvc.Testing;

namespace MyMinimalApi.Tests;

public class HelloWorldTests
{
    [Fact]
    public async Task TestRootEndpoint()
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();

        var response = await client.GetStringAsync("/");
        
        Assert.Equal("Hello World!", response);
    }
}