using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services for REST and gRPC
builder.Services.AddControllers();
builder.Services.AddGrpc();

// Uncomment to enable logging during runtime for debugging
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();

// Configure Kestrel for HTTP/1 and HTTP/2
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;

        // Uncommenting the below UseHttps() method will enable TLS
        // This is required to use tools like grpcurl to hit the grpc services
        // because grpcurl needs HTTP2. The following command should work on a local build:
        // grpcurl -insecure -proto Protos/Store.proto -d '{"key":"example"}' localhost:5000 Store.Get
        listenOptions.UseHttps(); // Enable HTTPS with a default certificate
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

// Map endpoints for REST and gRPC
app.MapControllers(); // REST API
app.MapGrpcService<GrpcStoreService>(); // gRPC service

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
