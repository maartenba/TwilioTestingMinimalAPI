using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MinimalAPI.Tests;

public class PeopleTests
{
    [Fact]
    public async Task CreatePerson()
    {
        await using var application = new WebApplicationFactory<Program>();

        var client = application.CreateClient();
        
        var result = await client.PostAsJsonAsync("/people", new Person 
        { 
            FirstName = "Maarten",
            LastName = "Balliauw", 
            Email = "maarten@jetbrains.com"
        });
        
        Assert.True(result.StatusCode == HttpStatusCode.OK);
        Assert.Equal("\"Hello, Maarten Balliauw\"", await result.Content.ReadAsStringAsync());
    }
    
    [Fact]
    public async Task CreatePersonValidatesObject()
    {
        await using var application = new WebApplicationFactory<Program>();

        var client = application.CreateClient();
        
        var result = await client.PostAsJsonAsync("/people", new Person());
        
        Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
        
        var validationResult = await result.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        Assert.NotNull(validationResult);
        Assert.Equal("The FirstName field is required.", validationResult!.Errors["FirstName"][0]);
    }
}