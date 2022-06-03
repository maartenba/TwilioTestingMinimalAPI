using System.ComponentModel.DataAnnotations;
using MiniValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IPeopleService, PeopleService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/people", (Person person, IPeopleService peopleService) =>
    !MiniValidator.TryValidate(person, out var errors)
        ? Results.ValidationProblem(errors)
        : Results.Ok(peopleService.Create(person)));

app.Run();

public partial class Program { }

public interface IPeopleService
{
    string Create(Person person);
}

public class PeopleService : IPeopleService
{
    public string Create(Person person) 
        => $"s{person.FirstName} {person.LastName} created.";
}

public class Person
{
    [Required, MinLength(2)]
    public string? FirstName { get; set; }

    [Required, MinLength(2)]
    public string? LastName { get; set; }

    [Required, DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
}