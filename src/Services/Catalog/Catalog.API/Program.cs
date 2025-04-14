using BuildingBlocks.Behaviors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
// Add services to container

var Assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(Assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(Assembly);

builder.Services.AddCarter();

builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);

}).UseLightweightSessions();



var app = builder.Build();

// Configure the HTTP request pipline
app.MapCarter();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
{

    var exeption = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    if (exeption is null)
    {
        return;
    }
    var problemDetails = new ProblemDetails
    {
        Title = exeption.Message,
        Status = StatusCodes.Status500InternalServerError,
        Detail = exeption.StackTrace
    };
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogError(exeption, exeption.Message);

    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    context.Response.ContentType = "application/problem+json";

    await context.Response.WriteAsJsonAsync(problemDetails);
});

});

app.Run();


// Add For BDD Test
public partial class Program { }
