using KeyValueStore.Core.Interfaces;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services for REST
builder.Services.AddControllers();

// Configure Kestrel for HTTP/1
builder.WebHost.ConfigureKestrel(options =>
{
    // HTTP/1.1 (for REST API) on port 5000
    options.ListenAnyIP(5000, listenOptions =>
    {
        // curl - k https://localhost:5001/weatherforecast
        listenOptions.Protocols = HttpProtocols.Http1;
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<IKeyValueStore, KeyValueStore.Core.Services.KeyValueStore>();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World!");

// Map endpoints for REST
app.MapControllers();

app.Run();
