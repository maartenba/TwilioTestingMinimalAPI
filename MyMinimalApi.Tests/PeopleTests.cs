using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MyMinimalApi.Tests;

public class TestPeopleService : IPeopleService
{
    public string Create(Person person) => "It works!";
}

class TestingApplication : WebApplicationFactory<Person>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services => 
        {
            services.AddScoped<IPeopleService, TestPeopleService>();
        });
 
        return base.CreateHost(builder);
    }
}

public class PeopleTests
{
    [Fact]
    public async Task CreatePerson()
    {
        await using var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder
                .ConfigureServices(services =>
                {
                    services.AddScoped<IPeopleService, TestPeopleService>();
                }));
        
        // or: await using var application = new TestingApplication();

        var client = application.CreateClient();
        
        var result = await client.PostAsJsonAsync("/people", new Person 
        { 
            FirstName = "Maarten",
            LastName = "Balliauw", 
            Email = "maarten@jetbrains.com"
        });
        
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("\"It works!\"", await result.Content.ReadAsStringAsync());
    }
    
    [Fact]
    public async Task CreatePersonValidatesObject()
    {
        await using var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder
                .ConfigureServices(services =>
                {
                    services.AddScoped<IPeopleService, TestPeopleService>();
                }));
        
        // or: await using var application = new TestingApplication();

        var client = application.CreateClient();
        
        var result = await client.PostAsJsonAsync("/people", new Person());
        
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        
        var validationResult = await result.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        Assert.NotNull(validationResult);
        Assert.Equal("The FirstName field is required.", validationResult!.Errors["FirstName"][0]);
    }
}