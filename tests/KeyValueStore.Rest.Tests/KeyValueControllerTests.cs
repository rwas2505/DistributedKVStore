using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using KeyValueStore.Core.Interfaces;
using KeyValueStore.Rest.Controllers;
using KeyValueStore.Rest.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace KeyValueStore.Rest.Tests;

public class KeyValueControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly Mock<IKeyValueStore> _storeMock;

    public KeyValueControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _storeMock = new Mock<IKeyValueStore>();
        _factory.SetupService(_storeMock);
    }

    [Fact]
    public async Task Get_KeyExists_ReturnsOkResultWithKeyValue()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";
        _storeMock.Setup(s => s.Get(key)).Returns(value);

        // Act
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"store/{key}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.Equal($"\"{value}\"", responseBody);
    }

    [Fact]
    public async Task Get_KeyDoesNotExist_ReturnsOkResultWithNullValue()
    {
        // Arrange
        var key = "test-key";
        _storeMock.Setup(s => s.Get(key)).Returns((string)null);

        // Act
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"store/{key}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.Equal("null", responseBody);
    }

    [Fact]
    public async Task Put_KeyAndValueProvided_ReturnsOkResultWithValue()
    {
        // Arrange
        var key = "test-key";
        var request = new PutRequestDto { Value = "test-value" };
        _storeMock.Setup(s => s.Put(key, request.Value)).Returns(request.Value);

        // Act
        var client = _factory.CreateClient();
        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"store/{key}", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.Equal($"\"{request.Value}\"", responseBody);
    }

    [Fact]
    public async Task Delete_KeyProvided_ReturnsOkResult()
    {
        // Arrange
        var key = "test-key";
        _storeMock.Setup(s => s.Delete(key));

        // Act
        var client = _factory.CreateClient();
        var response = await client.DeleteAsync($"store/{key}");

        // Assert
        response.EnsureSuccessStatusCode();
    }
}

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IKeyValueStore>(new Mock<IKeyValueStore>().Object);
        });
    }

    public void SetupService(Mock<IKeyValueStore> mockService)
    {
        var serviceProvider = Services.BuildServiceProvider();
        var serviceCollection = (IServiceCollection)serviceProvider.GetService(typeof(IServiceCollection));
        serviceCollection.Remove(typeof(IKeyValueStore));
        serviceCollection.AddSingleton(mockService.Object);
    }
}