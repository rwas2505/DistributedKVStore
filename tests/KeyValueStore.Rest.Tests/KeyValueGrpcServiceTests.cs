using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;
using KeyValueStore.Core.Interfaces;
using KeyValueStore.Core.Models;
using KeyValueStore.Grpc;
using KeyValueStore.Grpc.Services;
using System.Linq;

namespace KeyValueStore.Rest.Tests;


// Here is what I need: https://learn.microsoft.com/en-us/aspnet/core/grpc/test-services?view=aspnetcore-8.0#integration-test-grpc-services
public class StoreServiceTests : IClassFixture<StoreServiceTests.GrpcTestFixture>
{
    private readonly GrpcTestFixture _fixture;

    public StoreServiceTests(GrpcTestFixture fixture)
    {
        _fixture = fixture;
    }   

    [Fact]
    public async Task Get_KeyExists_ReturnsKeyValue()
    {
        // Arrange
        var key = "test-key-get-exists";
        var value = "test-value-get-exists";
        
        // Create a fresh mock for this test
        var storeMock = new Mock<IKeyValueStore>();
        storeMock
            .Setup(s => s.Get(key))
            .Returns(new GetResult { Exists = true, Value = value });

        // Override the store mock for this test
        _fixture.OverrideStoreMock(storeMock.Object);

        // Create the client
        var client = new Store.StoreClient(_fixture.Channel);

        // Act
        var response = await client.GetAsync(new KeyRequest { Key = key });

        // Assert
        Assert.True(response.Found);
        Assert.Equal(value, response.Value);

        // Verify that the mock was called
        storeMock.Verify(s => s.Get(key), Times.Once);
    }

    [Fact]
    public async Task Get_KeyDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var key = "test-key-get-not-exists";
        
        // Create a fresh mock for this test
        var storeMock = new Mock<IKeyValueStore>();
        storeMock
            .Setup(s => s.Get(key))
            .Returns(new GetResult { Exists = false, Value = null });

        // Override the store mock for this test
        _fixture.OverrideStoreMock(storeMock.Object);

        // Create the client
        var client = new Store.StoreClient(_fixture.Channel);

        // Act
        var response = await client.GetAsync(new KeyRequest { Key = key });

        // Assert
        Assert.False(response.Found);
        Assert.Empty(response.Value);

        // Verify that the mock was called
        storeMock.Verify(s => s.Get(key), Times.Once);
    }

    [Fact]
    public async Task Put_ValidKeyValue_Succeeds()
    {
        // Arrange
        var key = "test-key-put";
        var value = "test-value-put";
        
        // Create a fresh mock for this test
        var storeMock = new Mock<IKeyValueStore>();
        storeMock
            .Setup(s => s.Put(key, value))
            .Verifiable();

        // Override the store mock for this test
        _fixture.OverrideStoreMock(storeMock.Object);

        // Create the client
        var client = new Store.StoreClient(_fixture.Channel);

        // Act
        var response = await client.PutAsync(new PutRequest { Key = key, Value = value });

        // Assert
        Assert.NotNull(response);
        Assert.Contains("successfully", response.Message);

        // Verify that the mock was called
        storeMock.Verify(s => s.Put(key, value), Times.Once);
    }

    [Fact]
    public async Task Delete_ExistingKey_Succeeds()
    {
        // Arrange
        var key = "test-key-delete";
        var deletedValue = "deleted-test-value";
        
        // Create a fresh mock for this test
        var storeMock = new Mock<IKeyValueStore>();
        storeMock
            .Setup(s => s.Delete(key))
            .Returns(new DeleteResult { IsSuccess = true, DeletedValue = deletedValue })
            .Verifiable();

        // Override the store mock for this test
        _fixture.OverrideStoreMock(storeMock.Object);

        // Create the client
        var client = new Store.StoreClient(_fixture.Channel);

        // Act
        var response = await client.DeleteAsync(new DeleteRequest { Key = key });

        // Assert
        Assert.True(response.Deleted);
        Assert.Equal(deletedValue, response.DeletedValue);

        // Verify that the mock was called
        storeMock.Verify(s => s.Delete(key), Times.Once);
    }

    // Test fixture to manage the gRPC server lifecycle
    public class GrpcTestFixture : IDisposable
    {
        private ServiceProvider _serviceProvider;
        private TestServer _server;
        private IServiceCollection _services;
        private ILoggerFactory _loggerFactory;
        
        public GrpcChannel Channel { get; private set; }

        public GrpcTestFixture()
        {
            InitializeServer();
        }

        private void InitializeServer()
        {
            _services = new ServiceCollection();

            // Configure services for the test server
            _services.AddLogging(configure => 
                configure.AddConsole().SetMinimumLevel(LogLevel.Debug));
            
            // Add the actual service implementation
            _services.AddGrpc();
            _services.AddTransient<StoreService>();

            // Add a default mock that can be overridden
            var defaultStoreMock = new Mock<IKeyValueStore>().Object;
            _services.AddSingleton(defaultStoreMock);

            // Build service provider
            _serviceProvider = _services.BuildServiceProvider();
            _loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();

            // Create test server
            var builder = new WebHostBuilder()
                .ConfigureServices(config => 
                {
                    foreach (var service in _services)
                    {
                        config.Add(service);
                    }
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGrpcService<StoreService>();
                    });
                });

            _server = new TestServer(builder);

            // Create gRPC channel
            Channel = GrpcChannel.ForAddress("http://localhost", 
                new GrpcChannelOptions 
                { 
                    HttpClient = _server.CreateClient(),
                    LoggerFactory = _loggerFactory
                });
        }

        // Method to override the store mock for each test
        public void OverrideStoreMock(IKeyValueStore storeMock)
        {
            // Dispose old resources
            Channel.Dispose();
            _server.Dispose();
            _serviceProvider.Dispose();

            // Create a new service collection with all required services
            _services = new ServiceCollection();

            // Add logging
            _services.AddLogging(configure => 
                configure.AddConsole().SetMinimumLevel(LogLevel.Debug));
            
            // Add gRPC services
            _services.AddGrpc();
            _services.AddTransient<StoreService>();

            // Add the new mock
            _services.AddSingleton(storeMock);

            // Create a new service provider
            _serviceProvider = _services.BuildServiceProvider();
            _loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();

            // Create a new test server
            var builder = new WebHostBuilder()
                .ConfigureServices(config => 
                {
                    foreach (var service in _services)
                    {
                        config.Add(service);
                    }
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGrpcService<StoreService>();
                    });
                });

            _server = new TestServer(builder);

            // Create gRPC channel
            Channel = GrpcChannel.ForAddress("http://localhost", 
                new GrpcChannelOptions 
                { 
                    HttpClient = _server.CreateClient(),
                    LoggerFactory = _loggerFactory
                });
        }

        public void Dispose()
        {
            Channel.Dispose();
            _server.Dispose();
            _serviceProvider.Dispose();
            _loggerFactory.Dispose();
        }
    }
}