using Moq;
using Xunit;
using KeyValueStore.Core.Interfaces;
using KeyValueStore.Rest.Models;
using System.Text;
using System.Net.Http.Json;

namespace KeyValueStore.Rest.Tests;

public class KeyValueControllerTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly Mock<IKeyValueStore> _storeMock;
    private readonly HttpClient _client;

    public KeyValueControllerTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _storeMock = new Mock<IKeyValueStore>();
        _factory.SetupService(_storeMock);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Get_KeyExists_ReturnsOkResultWithKeyValue()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";
        _storeMock.Setup(s => s.Get(key)).Returns(new GetResult { Value = value });

        // Act
        var response = await _client.GetAsync($"store/{key}");

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
        _storeMock.Setup(s => s.Get(key)).Returns(new GetResult{ Value = null });

        // Act
        var response = await _client.GetAsync($"store/{key}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.Equal("null", responseBody);
    }

    [Fact]
    public async Task Put_KeyAndValueProvided_IsNotUpdate_ReturnsSuccessAndNotUpdate()
    {
        // Arrange
        var key = Guid.NewGuid().ToString();
        var request = new PutRequestDto("test-value");
        _storeMock.Setup(s => s.Put(key, request.Value)).Returns(new Core.Models.PutResult{ IsSuccess = true, IsUpdate = false });

        // Act
        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"store/{key}", content);

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
        var response = await _client.DeleteAsync($"store/{key}");

        // Assert
        response.EnsureSuccessStatusCode();
    }
}