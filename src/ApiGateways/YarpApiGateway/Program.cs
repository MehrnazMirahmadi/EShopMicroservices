var builder = WebApplication.CreateBuilder(args);
// Add services to the container
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy")); 
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

// Configure the HTTP request pipline
app.MapReverseProxy();
app.Run();
