using KeyValueStore.Grpc;
using KeyValueStore.Grpc.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

// To run from CLI: dotnet run --project .\KeyValueStore.Grpc

// To test kv store grpc: 
// grpcurl --insecure --proto Protos/Store.proto -d '{"key":"example"}' localhost: 5001 Store.Get



// .Net default greeter service

// To list services: grpcurl --insecure --proto Protos/greet.proto localhost:5001 list
// --> returns greet.Greeter
// To list methods: grpcurl --insecure --proto Protos/greet.proto localhost:5001 list greet.Greeter
// --> returns greet.Greeter.SayHello
// To describe a method: grpcurl --insecure --proto Protos/greet.proto localhost:5001 describe greet.Greeter.SayHello
// --> returns greet.Greeter.SayHello is a method:
// Sends a greeting
// rpc SayHello( .greet.HelloRequest ) returns( .greet.HelloReply);
// To describe a type: grpcurl --insecure --proto Protos/greet.proto localhost:5001 describe greet.HelloRequest
// --> returns greet.HelloRequest is a message:
// The request message containing the user's name.
// message HelloRequest
//{
//  string name = 1;
//}
// To invoke method: grpcurl--insecure--proto Protos/greet.proto -d '{"name": "John"}' localhost:5001 greet.Greeter.SayHello
// --> returns {"message": "Hello John"}




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

// Configure Kestrel for HTTP/2
builder.WebHost.ConfigureKestrel(options =>
{
    // HTTP/2 with TLS (for gRPC) on port 5001
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
        listenOptions.UseHttps(); // Enable HTTPS for gRPC
    });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<StoreService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
