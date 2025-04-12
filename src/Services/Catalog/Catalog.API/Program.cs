var builder = WebApplication.CreateBuilder(args);
// Add services to container
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);

}).UseLightweightSessions();



var app = builder.Build();

// Configure the HTTP request pipline
app.MapCarter();

app.Run();
// Add For Test
public partial class Program { }
